using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Web.Interfaces;
using Handlr.Framework.Routing.Types;
using System;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that transforms a given Xml structure with a specific Xml Stylesheet
    /// </summary>
    [Tag("Translate")]
    public class XslTranslation : Translate
    {
        /// <summary>
        /// Creates a new XsltTransform instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public XslTranslation(IController executionContext) : base(executionContext) { }

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

            IFieldCache toTranslateCache;

            if (!string.IsNullOrEmpty(LoaderArguments.TemplateInputKey))
                toTranslateCache = new XmlFieldCache(LoaderArguments.TemplateInputKey, fieldCache[LoaderArguments.TemplateInputKey].ToString());
            else
                toTranslateCache = fieldCache;

            var translatorInstance = new Translators.XslTranslator();
            var loaderInstance = new Loaders.TranslateLoaderArguments(LoaderArguments.AbsolutePath, LoaderArguments.RelativePath, LoaderArguments.Configuration);

            translatorInstance.Load(loaderInstance);

            var translatedCache = translatorInstance.Translate(toTranslateCache);

            if (!string.IsNullOrEmpty(LoaderArguments.TemplateOutputKey))
                fieldCache.Add(LoaderArguments.TemplateOutputKey, translatedCache[LoaderArguments.TemplateOutputKey]);
            else
                fieldCache.AddRange(translatedCache);
            return fieldCache;
        }
    }
}
