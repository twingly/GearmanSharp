using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Twingly.Gearman.Configuration
{
    public sealed class GearmanConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("clusters", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ClustersConfigurationElementCollection))]
        public ClustersConfigurationElementCollection Clusters
        {
            get { return (ClustersConfigurationElementCollection)this["clusters"]; }
            set { this["clusters"] = value; }
        }
    }
}
