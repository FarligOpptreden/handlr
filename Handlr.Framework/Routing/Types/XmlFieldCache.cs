using Handlr.Framework.Routing.Interfaces;
using System.Collections.Generic;
using System;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents a Xml construct that can be passed to a translator.
    /// </summary>
    public class XmlFieldCache : Dictionary<string, object>, IFieldCache, IInput
    {
        public XmlFieldCache(string key, string xml)
        {
            Add(key, xml.ToXml());
        }

        /// <summary>
        /// Checks to see whether the given field name exists in the cache or not.
        /// </summary>
        /// <param name="name">The name of the field to search in the cache</param>
        /// <returns>A value indicating whether the field exists in the cache or not</returns>
        public bool Exists(string name)
        {
            return ContainsKey(name);
        }

        /// <summary>
        /// Returns a list of all keys in the dictionary.
        /// </summary>
        /// <returns>IEnumerable of type string</returns>
        public IEnumerable<string> GetKeys()
        {
            return Keys as IEnumerable<string>;
        }

        /// <summary>
        /// Gets the specified field's value.
        /// </summary>
        /// <typeparam name="T">The type of field to return</typeparam>
        /// <param name="name">The name of the field to return</param>
        /// <returns></returns>
        public T GetValue<T>(string name)
        {
            return (T)Convert.ChangeType(this[name], typeof(T));
        }
    }
}
