using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Routing.Loaders;
using System;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Web.Interfaces;
using System.IO;
using Handlr.Framework.Routing.Exceptions;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that transforms a given Xml structure with a specific Xml Stylesheet
    /// </summary>
    [Tag("XsltTransform")]
    public class XsltTransform : Base<XsltTransformLoaderArguments>
    {
        /// <summary>
        /// Creates a new XsltTransform instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public XsltTransform(IController executionContext) : base(executionContext) { }

        /// <summary>
        /// Performs the transformation of the XML
        /// executed in order.
        /// </summary>
        /// <param name="fieldCache">The field cache to use during the conditional check</param>
        /// <returns>An updated field cache containing data returned by all child steps</returns>
        /// <exception cref="ArgumentNullException">Thrown when the fieldCache parameter is null</exception>
        public override IFieldCache ExecuteStep(IFieldCache fieldCache)
        {
            if (fieldCache == null)
                throw new ArgumentNullException("fieldCache");
            string path = LoaderArguments.TemplatePath.Replace("/", "\\").Replace("{AbsolutePath}", LoaderArguments.AbsolutePath);
            FileInfo fi = new FileInfo(path);

            if (!fi.Exists)
                throw new ParserException(string.Format("The translation file \"{0}\" could not be loaded as the file does not exist", path));

            string transformedXml;
            XslCompiledTransform translator = new XslCompiledTransform();
            try
            {
                using (XmlReader stylesheetReader = XmlReader.Create(path))
                {
                    translator.Load(stylesheetReader);
                }
            }
            catch (Exception ex)
            {
                throw new ParserException(string.Format("The Xsl file \"{0}\" could not be loaded: {1}", path, ex.Message));
            }

            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    translator.Transform(XDocument.Parse(fieldCache[LoaderArguments.TemplateInputKey].ToString()).CreateReader(), null, sw);
                    transformedXml = sw.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new ParserException(string.Format("The Xml could not be transformed: {0}", ex.Message));
            }
            fieldCache.AddRange(new Dictionary<string, object>() { { LoaderArguments.TemplateOutputKey, transformedXml } });
            return fieldCache;
        }
    }
}
