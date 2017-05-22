using System;
using System.Xml.Linq;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents the loader arguments for an XSLT translation step.
    /// </summary>
    public class XsltTranslationLoaderArguments : TranslationLoaderArguments
    {
        /// <summary>
        /// Gets the path to the template file.
        /// </summary>
        public string TemplateDataKey { get; private set; }

        /// <summary>
        /// Creates a new XsltTranslationLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public XsltTranslationLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            var dataKeyAttr = configuration.Attribute("dataKey");

            if (dataKeyAttr == null)
                throw new ArgumentException("The dataKey attribute for the translation was not specified.");

            TemplateDataKey = dataKeyAttr.Value;

            if (string.IsNullOrEmpty(TemplateDataKey))
                throw new ArgumentException("The dataKey attribute for the translation does not have a value specified.");
        }
    }
}
