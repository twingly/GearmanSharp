using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Twingly.Gearman.Tests
{
    [TestFixture]
    public class GearmanClientTests
    {
        [Test]
        public void can_submit_backgroundjob()
        {
            var client = new GearmanClient();
            client.AddServer(Helpers.TestServerHost, Helpers.TestServerPort);
            var jobRequest = client.SubmitBackgroundJob("reverse", Encoding.ASCII.GetBytes("Hello World"));

            Assert.IsNotNull(jobRequest);
            Assert.IsNotNull(jobRequest.JobHandle);
        }

        [Test]
        public void can_fetch_jobstatus()
        {
            var client = new GearmanClient();
            client.AddServer(Helpers.TestServerHost, Helpers.TestServerPort);
            var jobRequest = client.SubmitBackgroundJob("reverse", Encoding.ASCII.GetBytes("Hello World"));
            var jobStatus = client.GetStatus(jobRequest);

            Assert.IsNotNull(jobStatus);
            // We can't safely assert that jobStatus.IsKnown is true, but it most likely should be.
        }
    }
}