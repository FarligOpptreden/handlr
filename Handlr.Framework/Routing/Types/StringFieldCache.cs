﻿using Handlr.Framework.Routing.Interfaces;
using System.Collections.Generic;
using System;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents a standard string input that can be passed to a translator.
    /// </summary>
    public class StringFieldCache : Dictionary<string, string>, IFieldCache, IInput
    {
        public StringFieldCache(string input)
        {
            Add("input", input);
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
