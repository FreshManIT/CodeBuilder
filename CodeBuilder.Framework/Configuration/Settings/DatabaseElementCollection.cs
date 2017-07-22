using System.Configuration;
using CodeBuilder.Configuration;

namespace CodeBuilder.Framework.Configuration.Settings
{
    /// <summary>
    /// Data base config
    /// </summary>
    [ConfigurationCollection(typeof(DatabaseElement), AddItemName = "database")]
    public sealed class DatabaseElementCollection : ConfigurationElementCollection
    {
        public new DatabaseElement this[string name]
        {
            get { return (DatabaseElement)BaseGet(name); }
            set
            {
                if (BaseGet(name) != null)
                {
                    int index = BaseIndexOf(BaseGet(name));
                    BaseRemoveAt(index);
                    BaseAdd(index, value);
                    return;
                }
                BaseAdd(value);
            }
        }

        /// <summary>
        /// Get data base element by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DatabaseElement this[int index]
        {
            get { return (DatabaseElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Add element
        /// </summary>
        /// <param name="element"></param>
        public void Add(DatabaseElement element)
        {
            this[element.Name] = element;
        }

        /// <summary>
        /// Remove element.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (BaseGet(key) != null)
                BaseRemove(key);
        }

        /// <summary>
        /// Create new element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DatabaseElement();
        }

        /// <summary>
        /// Get element by key
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DatabaseElement)element).Name;
        }
    }
}
