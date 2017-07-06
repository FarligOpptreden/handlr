using System;
using System.Collections.Generic;
using System.Linq;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework;
using System.Web;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents a cache of POST fields that is used throughout a process.
    /// </summary>
    public class PostFieldCache : Dictionary<string, string>, IFieldCache, IOutput, IInput
    {
        /// <summary>
        /// Creates a new PostFieldCache instance.
        /// </summary>
        public PostFieldCache() { }

        /// <summary>
        /// Creates a new PostFieldCache instance.
        /// </summary>
        /// <param name="body">The body representing the post</param>
        public PostFieldCache(string body)
        {
            if (string.IsNullOrEmpty(body))
                throw new ArgumentNullException("body");

            try
            {
                Dictionary<string, string> values =
                    (
                        from kvp in body.Split('&')
                        let split = kvp.Split('=')
                        select new
                        {
                            Key = split[0],
                            Value = split[1]
                        }
                    ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                this.AddRange(values);
            }
            catch
            {
                throw new ArgumentException("The supplied string is not a valid post body");
            }
        }

        /// <summary>
        /// Gets or sets a field's value based on the specified name.
        /// </summary>
        /// <param name="name">The name of the field to get or set</param>
        /// <returns>The value of the specified field</returns>
        /// <exception cref="ArgumentNullException">Thrown when the name indexer is null</exception>
        public new string this[string name]
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

        public override string ToString()
        {
            return
                (
                    from kvp in this
                    select HttpUtility.UrlEncode(kvp.Key) + "=" + HttpUtility.UrlEncode(kvp.Value)
                ).Flatten("&");
        }
    }
}
