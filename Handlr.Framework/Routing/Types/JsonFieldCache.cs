using Handlr.Framework.Routing.Interfaces;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents a JSON construct that can be passed to a translator.
    /// </summary>
    public class JsonFieldCache : Dictionary<string, object>, IFieldCache, IInput
    {
        public JsonFieldCache(string json)
        {
            var converted = JsonConvert.DeserializeObject(json);
            Add("json", converted is string ? converted : json.ToDictionary());
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
            return (T)Convert.ChangeType(this[name], typeof(T));
        }
    }
}
