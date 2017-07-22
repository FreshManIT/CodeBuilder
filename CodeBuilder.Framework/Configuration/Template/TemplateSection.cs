using System.Configuration;

namespace CodeBuilder.Configuration
{
    public class TemplateSection : ConfigurationSection
    {
        [ConfigurationProperty("templates", IsRequired = true)]
        public TemplateElementCollection Templates
        {
            get { return (TemplateElementCollection)base["templates"]; }
        }
    }
}
