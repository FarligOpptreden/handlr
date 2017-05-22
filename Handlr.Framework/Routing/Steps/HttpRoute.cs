using Handlr.Framework.Data;
using Handlr.Framework.Data.Http;
using Handlr.Framework.Web.Types;
using System;
using System.Collections.Generic;
using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Routing.Loaders;
using Handlr.Framework.Web.Interfaces;
using System.Linq;
using Handlr.Framework.Routing.Types;
using Handlr.Framework;
using Handlr.Framework.Routing.Exceptions;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that executes an HTTP call.
    /// </summary>
    [Tag("Route")]
    public class HttpRoute : Base<HttpRouteLoaderArguments, RestFieldCache>
    {
        /// <summary>
        /// Creates a new HttpRoute instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public HttpRoute(IController executionContext) : base(executionContext) { }

        private Handlr.Framework.Data.Http.Connector GetConnector()
        {
            switch (LoaderArguments.Method)
            {
                case Method.Get:
                    return Core.Factory<Get>(ParseValue(LoaderArguments.Url), null, true);
                case Method.Put:
                    return Core.Factory<Put>(ParseValue(LoaderArguments.Url), null, true);
                case Method.Post:
                    return Core.Factory<Post>(ParseValue(LoaderArguments.Url), null, true);
                case Method.Delete:
                    return Core.Factory<Delete>(ParseValue(LoaderArguments.Url), null, true);
            }

            throw new ArgumentException(string.Format("The request method {0} is not supported.", LoaderArguments.Method.ToString().ToUpper()));
        }

        /// <summary>
        /// Executes the HTTP call, transforming the supplied field cache to the format required 
        /// for the consuming service and transforming the resulting data back to the format 
        /// required for the field cache.
        /// </summary>
        /// <param name="fieldCache">The field cache to derive query parameters and post variables from</param>
        /// <returns>An updated field cache containing data returned by the HTTP call</returns>
        /// <exception cref="ArgumentNullException">Thrown when the fieldCache parameter is null</exception>
        /// <exception cref="ArgumentException">Thrown when the request method is not supported</exception>
        public override RestFieldCache ExecuteStep(RestFieldCache fieldCache)
        {
            if (fieldCache == null)
                throw new ArgumentNullException("fieldCache");

            string requestBody = null;
            if (LoaderArguments.PreTranslation != null)
            {
                try
                {
                    var translated = LoaderArguments.PreTranslation.Translate(fieldCache);

                    if (translated == null)
                        throw new ParserException(string.Format("The input to the HTTP call could not be parsed using the pre translation of type \"{0}\".", LoaderArguments.PreTranslation.GetType()));

                    requestBody = LoaderArguments.PreTranslation.ToString();
                }
                catch (Exception ex)
                {
                    throw new ParserException(string.Format("The input to the HTTP call could not be parsed using the pre translation of type \"{0}\": {1}", LoaderArguments.PreTranslation.GetType(), ex.Message));
                }
            }
            Dictionary<string, object> parameters = null; // TODO: Format the parameters
            Dictionary<string, string> headers =
                (
                    from h in LoaderArguments.Headers
                    select new
                    {
                        Key = h.Key,
                        Value = ParseValue(h.Value)
                    }
                ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            using (var db = GetConnector())
            {
                string result = db.Call(requestBody, parameters, headers);
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        RestFieldCache updatedCache = (RestFieldCache)LoaderArguments.PostTranslation.Translate(new JsonInput(result));
                        fieldCache.AddRange(updatedCache);
                    }
                    catch (Exception ex)
                    {
                        throw new ParserException(string.Format("The output from the HTTP call could not be parsed using the post translation of type \"{0}\": {1}", LoaderArguments.PostTranslation.GetType(), ex.Message));
                    }
                }
            }
            return fieldCache;
        }
    }
}
