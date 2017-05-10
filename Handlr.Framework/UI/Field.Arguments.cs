using System.Collections.Generic;

namespace Handlr.Framework.UI
{
    /// <summary>
    /// Exposes methods to render the various input fields.
    /// </summary>
    public static partial class Fields
    {
        /// <summary>
        /// Represents the attributes shared across all fields.
        /// </summary>
        public abstract class Arguments
        {
            /// <summary>
            /// Gets or sets the @id of the field.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the @label of the field.
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// Gets or sets the @value of the field.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets a value to indicate whether the field is readonly or not.
            /// </summary>
            public bool Readonly { get; set; }

            /// <summary>
            /// Gets or sets a value to indicate whether the field is disabled or not.
            /// </summary>
            public bool Disabled { get; set; }

            /// <summary>
            /// Gets or sets the @class of the field.
            /// </summary>
            public string Classes { get; set; }

            /// <summary>
            /// Gets or sets all the events of the field.
            /// </summary>
            public Events Events { get; set; }
        }

        /// <summary>
        /// Represents the attributes shared across all fields with text input ability.
        /// </summary>
        public abstract class InputArguments : Arguments
        {
            /// <summary>
            /// Gets or sets the @placeholder of the field.
            /// </summary>
            public string Placeholder { get; set; }
        }

        /// <summary>
        /// Represets the attributes of a text field.
        /// </summary>
        public class TextArguments : InputArguments
        {
            /// <summary>
            /// Gets or sets the @multiline of the field.
            /// </summary>
            public bool Multiline { get; set; }

            /// <summary>
            /// Gets or sets the @maxlength of the field.
            /// </summary>
            public int? MaxLength { get; set; }
        }

        /// <summary>
        /// Represents the arguments of a number field.
        /// </summary>
        public class NumberArguments : InputArguments { }

        /// <summary>
        /// Represents the arguments of a checkbox field.
        /// </summary>
        public class CheckboxArguments : Arguments { }

        /// <summary>
        /// Represents the arguments of a calendar field.
        /// </summary>
        public class CalendarArguments : InputArguments
        {
            /// <summary>
            /// Gets or sets the @format of the field.
            /// </summary>
            public string Format { get; set; }

            /// <summary>
            /// Gets or sets a value to indicate whether a time picker is also rendered for the field.
            /// </summary>
            public bool Time { get; set; }
        }

        /// <summary>
        /// Represents the arguments for a list field.
        /// </summary>
        public class ListArguments : InputArguments
        {
            /// <summary>
            /// Gets or sets the items to display in the list.
            /// </summary>
            public List<ListItem> Items { get; set; } = new List<ListItem>();
        }

        /// <summary>
        /// Represents the arguments for an autocomplete field.
        /// </summary>
        public class AutocompleteArguments : InputArguments
        {
            /// <summary>
            /// Gets or sets the HTML template to use for each item.
            /// </summary>
            public string Template { get; set; }

            /// <summary>
            /// Gets or sets the value binding.
            /// </summary>
            public ValueBinding ValueBinding { get; set; }

            /// <summary>
            /// Gets or sets the data binding.
            /// </summary>
            public DataBinding DataBinding { get; set; }
        }
    }
}
