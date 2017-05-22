using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Translates the specified REST field cache to a POST field cache.
    /// </summary>
    public class PostWriter : PostParser<RestFieldCache, PostFieldCache>
    {
        private PostFieldCache cache;

        /// <summary>
        /// Translates the supplied REST field cache to a POST field cache.
        /// </summary>
        /// <param name="input">The REST field cache to translate</param>
        /// <returns>A POST field cache containing all keys produced by the translation</returns>
        public override PostFieldCache Translate(RestFieldCache input)
        {
            cache = new PostFieldCache();
            cache.AddRange(LoadAndParseTemplate(input));
            return cache;
        }

        /// <summary>
        /// Gets a POST body string representation of the translated results.
        /// </summary>
        /// <returns>A POST body string</returns>
        public override string ToString()
        {
            return cache.ToString();
        }
    }
}
