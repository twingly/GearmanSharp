using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Twingly.Gearman.Configuration;

namespace Twingly.Gearman.Tests
{
    public static class Helpers
    {
        public static string TestServerHost;
        public static int TestServerPort;

        static Helpers()
        {
            var section = (GearmanConfigurationSection) ConfigurationManager.GetSection("gearman");
            var testCluster = section.Clusters["test"];
            var enumerator = testCluster.Servers.GetEnumerator();
            enumerator.MoveNext();
            var server = (ServerConfigurationElement)enumerator.Current;
            TestServerHost = server.Host;
            TestServerPort = server.Port;
        }
    }
}
