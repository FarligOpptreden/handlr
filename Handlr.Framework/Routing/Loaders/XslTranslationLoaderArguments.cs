using System;
using System.Xml.Linq;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents the loader arguments for an Xslt transformation step.
    /// </summary>
    public class XslTranslationLoaderArguments : TranslationLoaderArguments
    {
        /// <summary>
        /// Gets the Xml key to query
        /// </summary>
        public string TemplateInputKey { get; private set; }

        /// <summary>
        /// Gets the key to output the Xml to
        /// </summary>
        public string TemplateOutputKey { get; private set; }

        /// <summary>
        /// Creates a new XsltTransformLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public XslTranslationLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            var inputKeyAttr = configuration.Attribute("inputKey");
            var outputKeyAttr = configuration.Attribute("outputKey");

            if (inputKeyAttr == null)
                throw new ArgumentException("The inputKey attribute for the transformation was not specified.");

            if (outputKeyAttr == null)
                throw new ArgumentException("The outputKey attribute for the transformation was not specified.");

            TemplateInputKey = inputKeyAttr.Value;
            TemplateOutputKey = outputKeyAttr.Value;

            if (string.IsNullOrEmpty(TemplateInputKey))
                throw new ArgumentException("The inputKey attribute for the translation does not have a value specified.");

            if (string.IsNullOrEmpty(TemplateOutputKey))
                throw new ArgumentException("The outputKey attribute for the translation does not have a value specified.");
        }
    }
}
