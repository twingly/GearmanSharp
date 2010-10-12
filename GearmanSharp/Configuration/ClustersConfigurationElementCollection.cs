using System.Configuration;

namespace Twingly.Gearman.Configuration
{
    [ConfigurationCollection(typeof(ClusterConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public sealed class ClustersConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ClusterConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClusterConfigurationElement) element).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override string ElementName
        {
            get { return "cluster"; }
        }

        public ClusterConfigurationElement this[int index]
        {
            get { return (ClusterConfigurationElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public new ClusterConfigurationElement this[string name]
        {
            get { return (ClusterConfigurationElement)base.BaseGet(name); }
        }
    }
}