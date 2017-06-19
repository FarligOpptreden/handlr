using System;
using System.Collections.Generic;
using System.Linq;
using Handlr.Framework.Routing.Interfaces;
using System.Dynamic;
using System.Collections;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents a cache of fields that is used throughout a process.
    /// </summary>
    public class GenericFieldCache : Dictionary<string, object>, IFieldCache, IOutput, IInput
    {
        /// <summary>
        /// Creates a new GenericFieldCache instance.
        /// </summary>
        public GenericFieldCache() { }

        /// <summary>
        /// Creates a new GenericFieldCache instance.
        /// </summary>
        /// <param name="queryParams">The query parameters derived from the HTTP request</param>
        /// <param name="formVariables">The form variables derived from the HTTP request</param>
        /// <param name="pathVariables">The path variables derived from the HTTP request</param>
        public GenericFieldCache(Dictionary<string, object> queryParams, Dictionary<string, object> formVariables, Dictionary<string, object> pathVariables)
        {
            this
                .AddRange(queryParams)
                .AddRange(formVariables)
                .AddRange(pathVariables);
        }

        /// <summary>
        /// Creates a new GenericFieldCache from the specified dictionary.
        /// </summary>
        /// <param name="fieldCache">The dictionary to add to the new field cache</param>
        public GenericFieldCache(Dictionary<string, object> fieldCache)
        {
            this.AddRange(fieldCache);
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
        /// Gets the specified field's value.
        /// </summary>
        /// <typeparam name="T">The type of field to return</typeparam>
        /// <param name="name">The name of the field to return</param>
        /// <returns></returns>
        public T GetValue<T>(string name)
        {
            return (T)this[name];
        }
    }
}
