using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Represents a base class for building JSON readers or writers.
    /// </summary>
    public abstract class JsonParser : Parser
    {
        public JsonParser() : base ("[", "]", ",", "(?=,|\\s|\\r|\\n|}|\\))") { }

        /// <summary>
        /// Loads the translation file at the configured location and parses the supplied input according to the template.
        /// </summary>
        /// <param name="fieldCache">The field cache to parse in the template</param>
        /// <returns>A generic field cache representing the parsed template</returns>
        protected IFieldCache LoadAndParseTemplate(IFieldCache fieldCache)
        {
            string template = LoadTemplate();
            
            while (TryParseForLoop(template, fieldCache, out template)) { }
            while (TryParseDataMembers(template, fieldCache, out template)) { }

            GenericFieldCache parsed = null;
            try
            {
                parsed = new GenericFieldCache(template.ToDictionary());
            }
            catch { }

            if (string.IsNullOrEmpty(template) || parsed == null)
                throw new ParserException("The JSON translation could not be applied to the source data.");

            return parsed;
        }
    }
}
