using System;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Types;
using Handlr.Framework.Routing.Interfaces;
using System.Xml.XPath;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Translates the specified Xml string input to a standard generic field cache.
    /// </summary>
    public class XmlReader : XmlParser
    {
        /// <summary>
        /// Translates the supplied Xml string to a generic field cache.
        /// </summary>
        /// <param name="input">The Xml string to translate</param>
        /// <returns>A REST field cache containing all keys produced by the translation</returns>
        /// <exception cref="GenericFieldCache">Throw when the input string could not be parsed to Xml</exception>
        public override IFieldCache Translate(IFieldCache input)
        {
            try
            {
                input = new XmlFieldCache("xml", input["input"].ToString());
            }
            catch (Exception ex)
            {
                throw new ParserException("The input string could not be parsed to Xml: " + ex.Message);
            }
            var cache = new GenericFieldCache();
            var parsedTemplate = LoadAndParseTemplate(new GenericFieldCache((input as XmlFieldCache).ToDictionary()["xml"].ToDictionary()));
            cache.Add(parsedTemplate.Root.Name.LocalName, parsedTemplate.Root.XPathSelectElement("./*").ToString());
            return cache;
        }
    }
}
