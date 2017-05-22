using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Translates the specified REST field cache to a new REST field cache.
    /// </summary>
    public class JsonWriter : JsonParser<RestFieldCache, RestFieldCache>
    {
        private RestFieldCache cache;

        /// <summary>
        /// Translates the supplied REST field cache to a new REST field cache.
        /// </summary>
        /// <param name="input">The REST field cache to translate</param>
        /// <returns>A REST field cache containing all keys produced by the translation</returns>
        public override RestFieldCache Translate(RestFieldCache input)
        {
            cache = new RestFieldCache();
            cache.AddRange(LoadAndParseTemplate(input));
            return cache;
        }

        /// <summary>
        /// Gets a JSON representation of the translated results.
        /// </summary>
        /// <returns>A JSON string</returns>
        public override string ToString()
        {
            return cache.ToJson();
        }
    }
}
