using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Twingly.Gearman.Packets;

namespace Twingly.Gearman.Tests
{
    [TestFixture]
    public class GearmanConnectionTests
    {
        private string _gearmanHost = Helpers.TestServerHost;
        private int _gearmanPort = Helpers.TestServerPort;

        [Test]
        public void submitting_background_job_should_generate_job_created_response()
        {
            var connection = new GearmanConnection(_gearmanHost, _gearmanPort);
            connection.Connect();

            var jobReq = GearmanProtocol.PackRequest(PacketType.SUBMIT_JOB_BG, "reverse", Guid.NewGuid().ToString(), "Hello World");

            connection.SendPacket(jobReq);
            var response = connection.GetNextPacket();
            connection.Disconnect();


            Assert.AreEqual(PacketType.JOB_CREATED, response.Type);
            Assert.IsNotNull(GearmanProtocol.UnpackJobCreatedResponse(response));
        }

        [Test]
        public void should_be_able_to_send_can_do_request()
        {
            var connection = new GearmanConnection(_gearmanHost, _gearmanPort);
            connection.Connect();

            connection.SendPacket(GearmanProtocol.PackRequest(PacketType.CAN_DO, "reverse"));
            // Server won't send any response to CanDo.
            connection.Disconnect();


            //How do we assert that this worked?
        }

        [Test]
        public void requesting_job_for_function_without_pending_jobs_should_generate_no_job_response()
        {
            var connection = new GearmanConnection(_gearmanHost, _gearmanPort);
            connection.Connect();
            // tell the server which jobs we can receive, but randomize a function name.
            // we want to receive the NoJobResponse
            connection.SendPacket(GearmanProtocol.PackRequest(PacketType.CAN_DO, Guid.NewGuid().ToString()));


            connection.SendPacket(new RequestPacket(PacketType.GRAB_JOB));
            var response = connection.GetNextPacket();
            connection.Disconnect();


            Assert.AreEqual(PacketType.NO_JOB, response.Type);
        }

        [Test]
        public void requesting_job_for_function_with_pending_job_should_generate_job_assign_response()
        {
            var connection = new GearmanConnection(_gearmanHost, _gearmanPort);
            connection.Connect();
            // randomize a function name and argument, so it won't collide with other functions and we can assert on them
            var functionName = Guid.NewGuid().ToString();
            var functionArgument = Guid.NewGuid().ToString();
            var jobReq = GearmanProtocol.PackRequest(PacketType.SUBMIT_JOB_BG, functionName, Guid.NewGuid().ToString(), functionArgument);

            connection.SendPacket(jobReq);
            var jobCreatedResponse = connection.GetNextPacket();
            var jobHandle = GearmanProtocol.UnpackJobCreatedResponse(jobCreatedResponse);

            Debug.WriteLine(String.Format("Created job with handle '{0}' for function: {1}", jobHandle, functionName));
            connection.SendPacket(GearmanProtocol.PackRequest(PacketType.CAN_DO, functionName));


            connection.SendPacket(new RequestPacket(PacketType.GRAB_JOB));
            var response = connection.GetNextPacket();
            connection.Disconnect();


            Assert.AreEqual(PacketType.JOB_ASSIGN, response.Type);
            var jobAssignment = GearmanProtocol.UnpackJobAssignResponse(response);
            Assert.AreEqual(jobHandle, jobAssignment.JobHandle);
            Assert.AreEqual(functionName, jobAssignment.FunctionName);
            Assert.AreEqual(functionArgument, Encoding.ASCII.GetString(jobAssignment.FunctionArgument));
        }

        [Test]
        public void can_submit_job_and_grab_job_and_send_work_complete()
        {
            var connection = new GearmanConnection(_gearmanHost, _gearmanPort);
            connection.Connect();
            // randomize a function name and argument, so it won't collide with other functions and we can assert on them
            var functionName = Guid.NewGuid().ToString();
            var functionArgument = Guid.NewGuid().ToString();
            var jobReq = GearmanProtocol.PackRequest(PacketType.SUBMIT_JOB_BG, functionName, Guid.NewGuid().ToString(), functionArgument);

            connection.SendPacket(jobReq);
            var jobCreatedResponse = connection.GetNextPacket();
            var jobHandle = GearmanProtocol.UnpackJobCreatedResponse(jobCreatedResponse);
            Debug.WriteLine(String.Format("Created job with handle '{0}' for function: {1}", jobHandle, functionName));
            connection.SendPacket(GearmanProtocol.PackRequest(PacketType.CAN_DO, functionName));
            connection.SendPacket(new RequestPacket(PacketType.GRAB_JOB));
            var jobAssignment = GearmanProtocol.UnpackJobAssignResponse(connection.GetNextPacket());

            // Just return the argument as result
            var workCompleteRequest = GearmanProtocol.PackRequest(PacketType.WORK_COMPLETE, jobAssignment.JobHandle, jobAssignment.FunctionArgument);
            connection.SendPacket(workCompleteRequest);
            

            // will we receive any response from creating the job on the same connection? Seems not, but could it happen?


            // What can we assert here? That we won't get any more jobs perhaps?
            connection.SendPacket(new RequestPacket(PacketType.GRAB_JOB));
            var response = connection.GetNextPacket();
            connection.Disconnect();
            Assert.AreEqual(PacketType.NO_JOB, response.Type);
        }
    }
}