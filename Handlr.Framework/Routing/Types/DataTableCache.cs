using System;
using System.Collections.Generic;
using System.Linq;
using Handlr.Framework.Routing.Interfaces;
using System.Data;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents a cache of data tables that is used throughout a process.
    /// </summary>
    public class DataTableCache : Dictionary<string, object>, IFieldCache, IOutput, IInput
    {
        /// <summary>
        /// Creates a new DataTableCache instance.
        /// </summary>
        public DataTableCache() { }

        /// <summary>
        /// Creates a new DataTableCache instance.
        /// </summary>
        /// <param name="tables">The tables to add to the cache</param>
        public DataTableCache(List<DataTable> tables)
        {
            if (tables == null || tables.Count == 0)
                throw new ArgumentNullException("tables");

            try
            {
                for (int i = 1; i <= tables.Count; i++)
                    Add("Table" + i, tables[i - 1].ToDictionary()["Data"]);
            }
            catch
            {
                throw new ArgumentException("Could not add the supplied tables to the table cache");
            }
        }

        /// <summary>
        /// Gets or sets a field's value based on the specified name.
        /// </summary>
        /// <param name="name">The name of the field to get or set</param>
        /// <returns>The value of the specified field</returns>
        /// <exception cref="ArgumentNullException">Thrown when the name indexer is null</exception>
        public new object this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");

                return this
                    .Where(f => f.Key.ToLower() == name.ToLower())
                    .Select(f => f.Value)
                    .FirstOrDefault();
            }
            set
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");

                var key = this
                    .Where(f => f.Key.ToLower() == name.ToLower())
                    .Select(f => f.Key)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(key))
                    Add(name, value);
                else
                    base[key] = value;
            }
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
