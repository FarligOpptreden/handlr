using Handlr.Framework.Data;
using System;
using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Routing.Loaders;
using Handlr.Framework.Web.Interfaces;
using Handlr.Framework.Routing.Types;
using System.Collections.Generic;
using Handlr.Framework.Routing.Exceptions;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that executes an ADO database call.
    /// </summary>
    [Tag("Route")]
    public class AdoRoute : Base<AdoRouteLoaderArguments, RestFieldCache>
    {
        /// <summary>
        /// Creates a new AdoRoute instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public AdoRoute(IController executionContext) : base(executionContext) { }

        /// <summary>
        /// Executes the database call, transforming the resulting data using the translation 
        /// specified in the loader arguments.
        /// </summary>
        /// <param name="fieldCache">The field cache to derive parameters from</param>
        /// <returns>An updated field cache containing data returned by the database call</returns>
        /// <exception cref="ArgumentNullException">Throw when the fieldCache parameter is null</exception>
        public override RestFieldCache ExecuteStep(RestFieldCache fieldCache)
        {
            if (fieldCache == null)
                throw new ArgumentNullException("fieldCache");

            using (var db = Core.Factory<Ado>(ParseValue(LoaderArguments.ConnectionString), null, false, "", true))
            {
                var parameters = new Dictionary<string, object>();
                foreach (var parameter in LoaderArguments.Parameters)
                {
                    object value = ParseValue(parameter.Value.ToString());
                    switch (parameter.Type)
                    {
                        case AdoParameter.DataType.Bit:
                            if (value == null || string.IsNullOrEmpty(value.ToString()))
                            {
                                value = parameter.Nullable ? null : (object)false;
                                break;
                            }
                            bool outBool;
                            if (!bool.TryParse(value.ToString(), out outBool))
                                throw new ArgumentException("Invalid bit parameter passed to stored procedure", parameter.Name);
                            value = outBool;
                            break;
                        case AdoParameter.DataType.DateTime:
                            if (value == null || string.IsNullOrEmpty(value.ToString()))
                            {
                                value = parameter.Nullable ? null : (object)DateTime.Today;
                                break;
                            }
                            DateTime outDateTime;
                            if (!DateTime.TryParse(value.ToString(), out outDateTime))
                                throw new ArgumentException("Invalid datetime parameter passed to stored procedure", parameter.Name);
                            value = outDateTime;
                            break;
                        case AdoParameter.DataType.Float:
                            if (value == null || string.IsNullOrEmpty(value.ToString()))
                            {
                                value = parameter.Nullable ? null : (object)0.00f;
                                break;
                            }
                            float outFloat;
                            if (!float.TryParse(value.ToString(), out outFloat))
                                throw new ArgumentException("Invalid float parameter passed to stored procedure", parameter.Name);
                            value = outFloat;
                            break;
                        case AdoParameter.DataType.Int:
                            if (value == null || string.IsNullOrEmpty(value.ToString()))
                            {
                                value = parameter.Nullable ? null : (object)0;
                                break;
                            }
                            int outInt;
                            if (!int.TryParse(value.ToString(), out outInt))
                                throw new ArgumentException("Invalid int parameter passed to stored procedure", parameter.Name);
                            value = outInt;
                            break;
                        case AdoParameter.DataType.String:
                            if (value == null || string.IsNullOrEmpty(value.ToString()))
                            {
                                value = parameter.Nullable ? null : "";
                                break;
                            }
                            value = value.ToString();
                            break;
                    }
                    parameters.Add(parameter.Name, value);
                }
                var results = db.ExecuteReader(ParseValue(LoaderArguments.Query), parameters);
                try
                {
                    RestFieldCache updatedCache = (RestFieldCache)LoaderArguments.PostTranslation.Translate(new AdoInput(results));
                    fieldCache.AddRange(updatedCache);
                }
                catch (Exception ex)
                {
                    throw new ParserException(string.Format("The output from the ADO call could not be parsed using the post translation of type \"{0}\": {1}", LoaderArguments.PostTranslation.GetType(), ex.Message));
                }
            }
            return fieldCache;
        }
    }
}
