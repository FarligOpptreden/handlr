using Handlr.Framework.Routing.Attributes;
using System;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Web.Interfaces;
using Handlr.Framework.Routing.Types;
using System.Collections.Generic;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that transforms a given type to a specified type.
    /// </summary>
    [Tag("Translate")]
    public class JsonTranslation : Translate
    {
        /// <summary>
        /// Creates a new Transform instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public JsonTranslation(IController executionContext) : base(executionContext) { }

        /// <summary>
        /// Performs the transformation of the type.
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
            {
                string[] keyParts = LoaderArguments.TemplateInputKey.Split('.');
                var dataMember = Utilities.GetDataMember(keyParts, fieldCache);
                if (dataMember is Dictionary<string, object>)
                    toTranslateCache = new GenericFieldCache(dataMember as Dictionary<string, object>);
                else
                    toTranslateCache = new JsonFieldCache(dataMember.ToJson());
            }
            else
                toTranslateCache = fieldCache;

            var translatorInstance = new Translators.JsonWriter();
            var loaderInstance = new Loaders.TranslateLoaderArguments(LoaderArguments.AbsolutePath, LoaderArguments.RelativePath, LoaderArguments.Configuration);

            translatorInstance.Load(loaderInstance);

            var translatedCache = translatorInstance.Translate(toTranslateCache);

            if (!string.IsNullOrEmpty(LoaderArguments.TemplateOutputKey))
                fieldCache.Add(LoaderArguments.TemplateOutputKey, translatedCache);
            else
                fieldCache.AddRange(translatedCache);
            return fieldCache;
        }
    }
}
