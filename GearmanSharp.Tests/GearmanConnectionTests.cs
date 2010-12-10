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
        private string _gearmanHost = "127.0.0.1";
        private int _gearmanPort = 4730;

        [Test]
        public void submitting_background_job_should_generate_job_created_response()
        {
            var connection = new GearmanConnection(_gearmanHost, _gearmanPort);
            connection.Connect();


            var jobReq = new SubmitJobRequest("reverse", Encoding.ASCII.GetBytes("Hello World"),
                                              true, Guid.NewGuid().ToString(), GearmanJobPriority.Normal);
            connection.SendPacket(jobReq);
            var response = connection.GetNextPacket();
            connection.Disconnect();


            Assert.IsInstanceOfType(typeof(JobCreatedResponse), response);
            Assert.IsNotNull(((JobCreatedResponse)response).JobHandle);
        }

        [Test]
        public void should_be_able_to_send_can_do_request()
        {
            var connection = new GearmanConnection(_gearmanHost, _gearmanPort);
            connection.Connect();
            

            connection.SendPacket(new CanDoRequest("reverse"));
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
            connection.SendPacket(new CanDoRequest(Guid.NewGuid().ToString()));

            
            connection.SendPacket(new GrabJobRequest());
            var response = connection.GetNextPacket();
            connection.Disconnect();


            Assert.IsInstanceOfType(typeof(NoJobResponse), response);
        }

        [Test]
        public void requesting_job_for_function_with_pending_job_should_generate_job_assign_response()
        {
            var connection = new GearmanConnection(_gearmanHost, _gearmanPort);
            connection.Connect();
            // randomize a function name and argument, so it won't collide with other functions and we can assert on them
            var functionName = Guid.NewGuid().ToString();
            var functionArgument = Guid.NewGuid().ToString();
            var jobReq = new SubmitJobRequest(functionName, Encoding.ASCII.GetBytes(functionArgument),
                                              true, Guid.NewGuid().ToString(), GearmanJobPriority.Normal);
            connection.SendPacket(jobReq);
            var jobCreatedResponse = (JobCreatedResponse) connection.GetNextPacket();
            Debug.WriteLine(String.Format("Created job with handle '{0}' for function: {1}", jobCreatedResponse.JobHandle, functionName));
            connection.SendPacket(new CanDoRequest(functionName));


            connection.SendPacket(new GrabJobRequest());
            var response = connection.GetNextPacket();
            connection.Disconnect();


            Assert.IsInstanceOfType(typeof(JobAssignResponse), response);
            var jobAssignResponse = (JobAssignResponse) response;
            Assert.AreEqual(jobCreatedResponse.JobHandle, jobAssignResponse.JobHandle);
            Assert.AreEqual(functionName, jobAssignResponse.FunctionName);
            Assert.AreEqual(functionArgument, Encoding.ASCII.GetString(jobAssignResponse.FunctionArgument));
        }

        [Test]
        public void can_submit_job_and_grab_job_and_send_work_complete()
        {
            var connection = new GearmanConnection(_gearmanHost, _gearmanPort);
            connection.Connect();
            // randomize a function name and argument, so it won't collide with other functions and we can assert on them
            var functionName = Guid.NewGuid().ToString();
            var functionArgument = Guid.NewGuid().ToString();
            var jobReq = new SubmitJobRequest(functionName, Encoding.ASCII.GetBytes(functionArgument),
                                              true, Guid.NewGuid().ToString(), GearmanJobPriority.Normal);
            connection.SendPacket(jobReq);
            var jobCreatedResponse = (JobCreatedResponse)connection.GetNextPacket();
            Debug.WriteLine(String.Format("Created job with handle '{0}' for function: {1}", jobCreatedResponse.JobHandle, functionName));
            connection.SendPacket(new CanDoRequest(functionName));
            connection.SendPacket(new GrabJobRequest());
            var jobAssignResponse = (JobAssignResponse)connection.GetNextPacket();

            // Just return the argument as result
            var workCompleteRequest = new WorkCompleteRequest(jobAssignResponse.JobHandle, jobAssignResponse.FunctionArgument);
            connection.SendPacket(workCompleteRequest);
            

            // will we receive any response from creating the job on the same connection? Seems not, but could it happen?


            // What can we assert here? That we won't get any more jobs perhaps?
            connection.SendPacket(new GrabJobRequest());
            var response = connection.GetNextPacket();
            connection.Disconnect();
            Assert.IsInstanceOfType(typeof(NoJobResponse), response);
        }
    }
}