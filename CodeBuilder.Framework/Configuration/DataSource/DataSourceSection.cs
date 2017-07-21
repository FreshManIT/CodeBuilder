using System.Configuration;

namespace CodeBuilder.Configuration
{
    public class DataSourceSection : ConfigurationSection
    {
        [ConfigurationProperty("dataSources", IsRequired = true)]
        public DataSourceElementCollection DataSources
        {
            get { return (DataSourceElementCollection)base["dataSources"]; }
        }
    }
}
