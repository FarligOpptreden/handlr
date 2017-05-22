using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Exceptions;
using System.Xml.Linq;

namespace Handlr.Framework.Routing.Translators
{
    public abstract class XmlParser : Parser
    {
        public XmlParser() : base("", "", "", "(?=,|\\s|\\r|\\n|}|\\))") { }

        protected XDocument LoadAndParseTemplate(IFieldCache fieldCache)
        {
            string template = LoadTemplate();

            while (TryParseForLoop(template, fieldCache, out template)) { }
            while (TryParseDataMembers(template, fieldCache, out template)) { }

            XDocument parsed = null;
            try
            {
                parsed = template.ToXml();
            }
            catch { }

            if (string.IsNullOrEmpty(template) || parsed == null)
                throw new ParserException("The XML translation could not be applied to the source data.");

            return parsed;
        }
    }
}
