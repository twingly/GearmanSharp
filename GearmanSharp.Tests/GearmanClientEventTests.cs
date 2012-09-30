using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Twingly.Gearman.Tests
{
    [TestFixture]
    public class GearmanClientEventTests
    {
        [Test]
        public void can_get_job_event()
        {
            bool createFired = false;
            bool completeFired = false;
            bool completeData = false;
            var client = new GearmanClient();
            client.JobCreated += (o, e) => createFired = true;
            client.JobCompleted += (o, e) => completeFired = true;
            client.JobCompleted += (o, e) => completeData = (e.Data.SequenceEqual(Encoding.ASCII.GetBytes("dlroW olleH")));

            client.AddServer(Helpers.TestServerHost, Helpers.TestServerPort);
            var jobRequest = client.SubmitJob("reverse", Encoding.ASCII.GetBytes("Hello World"));

            Assert.IsTrue(createFired);
            Assert.IsTrue(completeFired);
            Assert.IsTrue(completeData);
        }
    }
}