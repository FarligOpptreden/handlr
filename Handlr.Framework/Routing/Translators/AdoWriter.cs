using Handlr.Framework.Routing.Types;
using System.Collections.Generic;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Translates the specified ADO input to a standard REST field cache.
    /// </summary>
    public class AdoWriter : JsonParser<AdoInput, RestFieldCache>
    {
        private RestFieldCache cache;

        /// <summary>
        /// Translates the supplied ADO input to a REST field cache.
        /// </summary>
        /// <param name="input">The ADO input to translate</param>
        /// <returns>A REST field cache containing all keys produced by the translation</returns>
        public override RestFieldCache Translate(AdoInput input)
        {
            Dictionary<string, object> tables = new Dictionary<string, object>();
            for (int i = 1; i <= input.Data.Count; i++)
                tables.Add("Table" + i, input.Data[i - 1].ToDictionary()["Data"]);
            cache = new RestFieldCache();
            cache.AddRange(LoadAndParseTemplate(tables));
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
