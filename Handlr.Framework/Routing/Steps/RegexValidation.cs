using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Handlr.Framework.Web.Interfaces;
using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Loaders;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that validates specific fields in the field cache against regex patterns.
    /// </summary>
    [Tag("Validate")]
    public class RegexValidation : Base<RegexValidationLoaderArguments, RestFieldCache>
    {
        private List<RegexValidationException> Exceptions = new List<RegexValidationException>();

        /// <summary>
        /// Creates a new RegexValidation instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public RegexValidation(IController executionContext) : base(executionContext) { }

        /// <summary>
        /// Executes the validations, throwing the necessary validations when the pattern matches
        /// fail.
        /// </summary>
        /// <param name="fieldCache">The field cache to use for pattern matching</param>
        /// <returns>The unmodified field cache</returns>
        /// <exception cref="RegexValidationException">Thrown when a regex pattern match fails</exception>
        public override RestFieldCache ExecuteStep(RestFieldCache fieldCache)
        {
            if (fieldCache == null)
                throw new ArgumentNullException("fieldCache");

            foreach (var field in LoaderArguments.Fields)
            {
                var cachedField = fieldCache[field.Name];
                if (cachedField == null)
                    cachedField = Utilities.GetDataMember(field.Name.Split('.'), fieldCache);
                if (cachedField == null)
                {
                    Exceptions.Add(new RegexValidationException(field.Name, "N/A", "N/A", "The field could not be validated because it does not exist in the field cache"));
                    continue;
                }
                if (!Regex.IsMatch(cachedField.ToString(), field.Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                    Exceptions.Add(new RegexValidationException(field.Name, cachedField, ParseValue(field.Regex), ParseValue(field.Message)));
            }

            if (Exceptions.Count > 0)
                throw new RegexValidationException(Exceptions);

            return fieldCache;
        }
    }
}