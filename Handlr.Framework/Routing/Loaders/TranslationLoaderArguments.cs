using System;
using System.Xml.Linq;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents the loader arguments for a translation step.
    /// </summary>
    public class TranslationLoaderArguments : Base
    {
        /// <summary>
        /// Gets the path to the template file.
        /// </summary>
        public string TemplatePath { get; private set; }

        /// <summary>
        /// Creates a new TranslationLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public TranslationLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            var pathAttr = configuration.Attribute("path");

            if (pathAttr == null)
                throw new ArgumentException("The path attribute for the translation was not specified.");

            TemplatePath = pathAttr.Value;

            if (string.IsNullOrEmpty(TemplatePath))
                throw new ArgumentException("The path attribute for the translation does not have a value specified.");
        }
    }
}
