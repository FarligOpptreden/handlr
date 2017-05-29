using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Translates the specified generic field cache to a new generic field cache.
    /// </summary>
    public class XmlWriter : XmlParser
    {
        private GenericFieldCache cache;

        /// <summary>
        /// Translates the supplied generic field cache to a new generic field cache.
        /// </summary>
        /// <param name="input">The generic field cache to translate</param>
        /// <returns>A REST field cache containing all keys produced by the translation</returns>
        public override IFieldCache Translate(IFieldCache input)
        {
            cache = new GenericFieldCache();
            var doc = LoadAndParseTemplate(input);
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
