using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Twingly.Gearman.Configuration;

namespace Twingly.Gearman.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        private const string TestConfigurationFilename = "TestConfiguration.config";

        [Test]
        public void can_read_configuration()
        {
            var fileMap = new ExeConfigurationFileMap {ExeConfigFilename = TestConfigurationFilename};
            
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            if (!File.Exists(config.FilePath))
                throw new ApplicationException(string.Format("The requested configuration file {0} does not exist!",
                                                             config.FilePath));

            var section = config.GetSection("gearman") as GearmanConfigurationSection;

            Assert.IsNotNull(section);
            Assert.IsNotNull(section.Clusters);
            Assert.IsNotEmpty(section.Clusters);

            foreach (ClusterConfigurationElement cluster in section.Clusters)
            {
                Assert.IsNotNull(cluster.Name);
                Assert.IsNotEmpty(cluster.Name);
                Assert.IsNotNull(cluster.Servers);
                Assert.IsNotEmpty(cluster.Servers);

                foreach (ServerConfigurationElement server in cluster.Servers)
                {
                    Assert.IsNotNull(server.Host);
                    Assert.IsNotEmpty(server.Host);

                    // Couldn't get the configuration validation to work.
                    Assert.Greater(server.Port, 0);
                    Assert.Less(server.Port, 65535);
                }
            }
        }
    }
}
