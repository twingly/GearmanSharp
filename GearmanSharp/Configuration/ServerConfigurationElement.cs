using System.Configuration;

namespace Twingly.Gearman.Configuration
{
    public sealed class ServerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get { return (string)this["host"]; }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        //[IntegerValidator(MinValue = 1, MaxValue = 65535)] // couldn't get this to work.
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }
    }
}