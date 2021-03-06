﻿using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Translates the specified data table input to a standard generic field cache.
    /// </summary>
    public class DataTableReader : JsonParser
    {
        private GenericFieldCache cache;

        /// <summary>
        /// Translates the supplied data table input to a generic field cache.
        /// </summary>
        /// <param name="input">The data table input to translate</param>
        /// <returns>A REST field cache containing all keys produced by the translation</returns>
        public override IFieldCache Translate(IFieldCache input)
        {
            cache = new GenericFieldCache();
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
