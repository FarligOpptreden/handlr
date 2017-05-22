using System.Collections.Generic;
using System.Xml.Linq;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents the loader arguments for initializing a regex validation.
    /// </summary>
    public class RegexValidationLoaderArguments : Base
    {
        /// <summary>
        /// Gets the fields to be validated.
        /// </summary>
        public List<Field> Fields { get; private set; } = new List<Field>();

        /// <summary>
        /// Creates a new RegexValidationLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public RegexValidationLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            foreach (XElement field in configuration.Elements())
                Fields.Add(new Field(
                    field.Attribute("name").Value,
                    field.Attribute("regex").Value,
                    field.Attribute("message").Value
                    ));
        }

        /// <summary>
        /// Represents a field being validated.
        /// </summary>
        public class Field
        {
            /// <summary>
            /// Gets the name of the field.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the regex pattern to use during validation.
            /// </summary>
            public string Regex { get; private set; }

            /// <summary>
            /// Gets the message to be displayed when the validation fails.
            /// </summary>
            public string Message { get; private set; }

            public Field(string name, string regex, string message)
            {
                Name = name;
                Regex = regex;
                Message = message;
            }
        }
    }
}
