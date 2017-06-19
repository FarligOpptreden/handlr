using System;
using System.Xml.Linq;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents the loader arguments for Transform step.
    /// </summary>
    public class TranslateLoaderArguments : TranslationLoaderArguments
    {
        /// <summary>
        /// Gets or sets the input key
        /// </summary>
        public string TemplateInputKey { get; private set; }

        /// <summary>
        /// Gets or sets the output key
        /// </summary>
        public string TemplateOutputKey { get; private set; }

        /// <summary>
        /// Creates a new TransformLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public TranslateLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            var inputKeyAttr = configuration.Attribute("inputKey");
            var outputKeyAttr = configuration.Attribute("outputKey");

            if (inputKeyAttr != null)
                TemplateInputKey = inputKeyAttr.Value;

            if (outputKeyAttr != null)
                TemplateOutputKey = outputKeyAttr.Value;
        }
    }
}
