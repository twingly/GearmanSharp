using System.Configuration;

namespace Twingly.Gearman.Configuration
{
    public sealed class ClusterConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("servers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServersConfigurationElementCollection))]
        public ServersConfigurationElementCollection Servers
        {
            get { return (ServersConfigurationElementCollection)this["servers"]; }
            set { this["servers"] = value; }
        }
    }
}