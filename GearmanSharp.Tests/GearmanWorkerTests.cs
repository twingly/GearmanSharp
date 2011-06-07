using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Twingly.Gearman.Packets;

namespace Twingly.Gearman.Tests
{
    [TestFixture]
    public class GearmanWorkerTests
    {
        [Test]
        public void will_create_the_correct_gearmanjob()
        {
            // TODO: This broke when we removed all specific packet classes.

            //var stubJobAssignResponse = MockRepository.GenerateStub<IJobAssignResponse>();
            //var stubConnection = MockRepository.GenerateStub<IGearmanConnection>();
            //var stubConnectionFactory = MockRepository.GenerateStub<IGearmanConnectionFactory>();

            //const string functionName = "func";
            //var functionArgument = new byte[3] { 1, 2, 3 };
            //const string jobHandle = "handle";

            //stubJobAssignResponse.Stub(p => p.JobHandle).Return(jobHandle);
            //stubJobAssignResponse.Stub(p => p.FunctionName).Return(functionName);
            //stubJobAssignResponse.Stub(p => p.FunctionArgument).Return(functionArgument);
            //stubJobAssignResponse.Stub(p => p.Type).Return(PacketType.JOB_ASSIGN);

            //stubConnection.Stub(conn => conn.IsConnected()).Return(true);
            //stubConnection.Stub(conn => conn.GetNextPacket())
            //    .Return(stubJobAssignResponse);

            //stubConnectionFactory.Stub(x => x.CreateConnection("host", 12345))
            //    .IgnoreArguments()
            //    .Return(stubConnection);

            //GearmanJobFunction<byte[], byte[]> func = delegate(IGearmanJob<byte[], byte[]> job) {
            //    Assert.IsNotNull(job);
            //    Assert.AreEqual(jobHandle, job.Info.JobHandle);
            //    Assert.AreEqual(functionName, job.Info.FunctionName);
            //    Assert.AreEqual(functionArgument, job.FunctionArgument);
            //};


            //var worker = new GearmanWorker {ConnectionFactory = stubConnectionFactory};

            //worker.AddServer("host", 12345);
            //worker.RegisterFunction(functionName, func);
            //worker.Work();
        }
    }
}
