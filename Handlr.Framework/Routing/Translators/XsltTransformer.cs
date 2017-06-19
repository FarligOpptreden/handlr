using System;
using Handlr.Framework.Routing.Loaders;
using Handlr.Framework.Routing.Interfaces;
using System.Xml.Linq;
using System.IO;
using Handlr.Framework.Routing.Exceptions;
using System.Xml.Xsl;
using System.Collections.Generic;

namespace Handlr.Framework.Routing.Translators
{
    public class XsltTransformer : Base<TransformLoaderArguments>
    {
        public override IFieldCache Translate(IFieldCache fieldCache)
        {
            string path = LoaderArguments.TemplatePath.Replace("/", "\\").Replace("{AbsolutePath}", LoaderArguments.AbsolutePath);
            FileInfo fi = new FileInfo(path);

            if (!fi.Exists)
                throw new ParserException(string.Format("The translation file \"{0}\" could not be loaded as the file does not exist", path));

            string transformedXml;
            XslCompiledTransform translator = new XslCompiledTransform();
            try
            {
                using (System.Xml.XmlReader stylesheetReader = System.Xml.XmlReader.Create(path))
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
