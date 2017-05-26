using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Routing.Loaders;
using System;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Web.Interfaces;
using System.IO;
using Handlr.Framework.Routing.Exceptions;
using System.Xml.Xsl;
using System.Xml;

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

            string resultXML;
            XslCompiledTransform translator = new XslCompiledTransform();
            using (StringReader sr = new StringReader(path))
            {
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    translator.Load(xr);
                }
            }            
            using (StringReader sr = new StringReader(fieldCache["ApiPolicies"].ToString()))
            {
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    using (StringWriter sw = new StringWriter())
                    {
                        translator.Transform(xr, null, sw);
                        resultXML = sw.ToString();
                    }
                }
            }
            return null;
        }
    }
}
