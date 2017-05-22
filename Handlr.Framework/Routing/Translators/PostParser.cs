using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Represents a base class for building POST body readers or writers.
    /// </summary>
    public abstract class PostParser : Parser
    {
        public PostParser() : base ("", "", "&", "(?=\\=|\\&|\\s|\\r|\\n|\\)|$)") { }

        /// <summary>
        /// Loads the translation file at the configured location and parses the supplied input according to the template.
        /// </summary>
        /// <param name="fieldCache">The field cache to parse in the template</param>
        /// <returns>A POST field cache representing the parsed template</returns>
        protected IFieldCache LoadAndParseTemplate(IFieldCache fieldCache)
        {
            string template = LoadTemplate();
            template = template.Replace("\r", "").Replace("\n", "");

            while (TryParseForLoop(template, fieldCache, out template)) { }
            while (TryParseDataMembers(template, fieldCache, out template)) { }

            PostFieldCache parsed = null;
            try
            {
                parsed = new PostFieldCache(template);
            }
            catch { }

            if (string.IsNullOrEmpty(template) || parsed == null)
                throw new ParserException("The POST translation could not be applied to the source data.");

            return parsed;
        }
    }
}
