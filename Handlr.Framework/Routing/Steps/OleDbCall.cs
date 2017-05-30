using Handlr.Framework.Data;
using System;
using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Web.Interfaces;
using Handlr.Framework.Routing.Types;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that executes an ADO database call.
    /// </summary>
    [Tag("DatabaseCall")]
    public class OleDbCall : DatabaseCall
    {
        /// <summary>
        /// Creates a new OleCall instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public OleDbCall(IController executionContext) : base(executionContext) { }

        /// <summary>
        /// Executes the database call, transforming the resulting data using the translation 
        /// specified in the loader arguments.
        /// </summary>
        /// <param name="fieldCache">The field cache to derive parameters from</param>
        /// <returns>An updated field cache containing data returned by the database call</returns>
        /// <exception cref="ArgumentNullException">Throw when the fieldCache parameter is null</exception>
        public override IFieldCache ExecuteStep(IFieldCache fieldCache)
        {
            if (fieldCache == null)
                throw new ArgumentNullException("fieldCache");

            using (var db = Core.Factory<OleDb>(ParseValue(LoaderArguments.ConnectionString) as string, null, false, "", true))
            {
                
                var results = db.ExecuteReader(ParseValue(LoaderArguments.Query) as string, DeriveParameters());
                try
                {
                    IFieldCache updatedCache = LoaderArguments.OutputTranslation.Translate(new DataTableCache(results));
                    fieldCache.AddRange(updatedCache);
                }
                catch (Exception ex)
                {
                    throw new ParserException(string.Format("The output from the OLEDB call could not be parsed using the post translation of type \"{0}\": {1}", LoaderArguments.OutputTranslation.GetType(), ex.Message));
                }
            }
            return fieldCache;
        }
    }
}
