using System;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Translates the specified JSON string input to a standard REST field cache.
    /// </summary>
    public class JsonReader : JsonParser<StringInput, RestFieldCache>
    {
        /// <summary>
        /// Translates the supplied JSON string to a REST field cache.
        /// </summary>
        /// <param name="input">The JSON string to translate</param>
        /// <returns>A REST field cache containing all keys produced by the translation</returns>
        /// <exception cref="RestFieldCache">Throw when the input string could not be parsed to JSON</exception>
        public override RestFieldCache Translate(StringInput input)
        {
            try
            {
                input = new JsonInput(input.Input);
            }
            catch (Exception ex)
            {
                throw new ParserException("The input string could not be parsed to JSON: " + ex.Message);
            }
            var cache = new RestFieldCache();
            cache.AddRange(LoadAndParseTemplate((input as JsonInput).Json));
            return cache;
        }
    }
}
