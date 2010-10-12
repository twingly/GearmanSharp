using System.Configuration;

namespace Twingly.Gearman.Configuration
{
    [ConfigurationCollection(typeof(ServerConfigurationElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public sealed class ServersConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var serverElement = (ServerConfigurationElement)element;
            return string.Format("{0}:{1}", serverElement.Host, serverElement.Port);
        }

        protected override string ElementName
        {
            get { return "server"; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
    }
}