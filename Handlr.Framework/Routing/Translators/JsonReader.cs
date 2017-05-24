using System;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Types;
using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Translates the specified JSON string input to a standard generic field cache.
    /// </summary>
    public class JsonReader : JsonParser
    {
        /// <summary>
        /// Translates the supplied JSON string to a generic field cache.
        /// </summary>
        /// <param name="input">The JSON string to translate</param>
        /// <returns>A REST field cache containing all keys produced by the translation</returns>
        /// <exception cref="GenericFieldCache">Throw when the input string could not be parsed to JSON</exception>
        public override IFieldCache Translate(IFieldCache input)
        {
            try
            {
                input = new JsonFieldCache(input["input"].ToString());
            }
            catch (Exception ex)
            {
                throw new ParserException("The input string could not be parsed to JSON: " + ex.Message);
            }
            var cache = new GenericFieldCache();
            cache.AddRange(LoadAndParseTemplate(new GenericFieldCache((input as JsonFieldCache).ToDictionary())));
            return cache;
        }
    }
}
