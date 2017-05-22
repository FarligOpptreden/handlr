using System;
using System.Collections.Generic;

namespace Handlr.Framework.Routing.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a regex validation fails.
    /// </summary>
    public class RegexValidationException : Exception
    {
        private string _Message;

        /// <summary>
        /// Gets the field that failed the validation.
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// Gets the value of the field that failed the validation.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the message to be displayed for the failed validation.
        /// </summary>
        public string ValidationMessage { get; private set; }

        /// <summary>
        /// Gets the regex pattern that was tested during the validation.
        /// </summary>
        public string ValidationPattern { get; private set; }

        /// <summary>
        /// Gets a list of child validations that failed.
        /// </summary>
        public List<RegexValidationException> ValidationExceptions { get; private set; }

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        public override string Message
        {
            get
            {
                if (!string.IsNullOrEmpty(_Message))
                    return _Message;
                return string.Format("Field \"{0}\" with value \"{1}\" did not validate successfully against the pattern \"{2}\": {3}", Field, Value.ToString(), ValidationPattern, ValidationMessage);
            }
        }

        /// <summary>
        /// Creates a new RegexValidationException instance.
        /// </summary>
        /// <param name="validationExceptions">The list of child exceptions to associate with the exception</param>
        /// <exception cref="ArgumentNullException">Thrown when the validationExceptions parameter is null</exception>
        public RegexValidationException(List<RegexValidationException> validationExceptions) : base()
        {
            if (validationExceptions == null)
                throw new ArgumentNullException("validationExceptions");

            _Message = string.Format("{0} field{1} did not validate successfully. Please check the ValidationExceptions property for more details.", validationExceptions.Count, validationExceptions.Count == 1 ? "" : "s");
            ValidationExceptions = validationExceptions;
        }

        /// <summary>
        /// Creates a new RegexValidationException instance.
        /// </summary>
        /// <param name="message">The message to display for the exception</param>
        /// <param name="validationExceptions">The list of child exceptions to associate with the exception</param>
        public RegexValidationException(string message, List<RegexValidationException> validationExceptions) : base()
        {
            _Message = message;
            ValidationExceptions = validationExceptions;
        }

        /// <summary>
        /// Creates a new RegexValidationException instance.
        /// </summary>
        /// <param name="field">The field that caused the exception</param>
        /// <param name="value">The field's value that caused the exception</param>
        /// <param name="pattern">The regex pattern that caused the exception</param>
        /// <param name="message">The message to display for the exception</param>
        public RegexValidationException(string field, object value, string pattern, string message) : base()
        {
            Field = string.IsNullOrEmpty(field) ? "<undefined>" : field;
            Value = value == null ? "<undefined>" : value;
            ValidationPattern = string.IsNullOrEmpty(pattern) ? "<undefined>" : pattern;
            ValidationMessage = string.IsNullOrEmpty(message) ? "<undefined>" : message;
        }
    }
}
