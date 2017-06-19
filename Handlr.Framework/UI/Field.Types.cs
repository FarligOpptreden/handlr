using Handlr.Framework.Web.Types;
using System.Collections.Generic;

namespace Handlr.Framework.UI
{
    /// <summary>
    /// Exposes methods to render the various input fields.
    /// </summary>
    public static partial class Fields
    {
        /// <summary>
        /// Represents an individual item in a list field.
        /// </summary>
        public class ListItem
        {
            public ListItem() { }

            public ListItem(string display, string value)
            {
                Display = display;
                Value = value;
            }

            /// <summary>
            /// Gets or sets the value of the item.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets the display of the item.
            /// </summary>
            public string Display { get; set; }
        }

        /// <summary>
        /// Represents a value binding of an autocomplete field.
        /// </summary>
        public class ValueBinding
        {
            /// <summary>
            /// Gets or sets the @value of the binding.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets the @display of the binding.
            /// </summary>
            public string Display { get; set; }

            /// <summary>
            /// Gets or sets the @context of the binding.
            /// </summary>
            public string Context { get; set; }
        }

        /// <summary>
        /// Represents the data binding of an autocomplete field.
        /// </summary>
        public class DataBinding
        {
            /// <summary>
            /// Gets or sets the URL to fetch the data from.
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// Gets or sets the access method to use when retrieving the data.
            /// </summary>
            public Method Method { get; set; }

            /// <summary>
            /// Gets or sets the parameters to send when retrieving the data.
            /// </summary>
            public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

            /// <summary>
            /// Gets or sets the field accessor if the data returned is not of type array.
            /// </summary>
            public string FieldAccessor { get; set; }

            /// <summary>
            /// Gets or sets the events associated with the data binding.
            /// </summary>
            public DataBindingEvents Events { get; set; }
        }

        /// <summary>
        /// Represents the HTML content of the template container.
        /// </summary>
        public class Template
        {
            /// <summary>
            /// Gets or sets the tempalte css class.
            /// </summary>
            public string Classes { get; set; }

            /// <summary>
            /// Gets or sets the template content.
            /// </summary>
            public string Content { get; set; }
        }

        /// <summary>
        /// Represents the validation for the field.
        /// </summary>
        public class Validation
        {
            /// <summary>
            /// Gets or sets the clientside method to invoke.
            /// </summary>
            public string OnValidate { get; set; }

            /// <summary>
            /// Gets or sets the validation message to display.
            /// </summary>
            public string Message { get; set; }
        }

        /// <summary>
        /// Represents the events shared across all fields.
        /// </summary>
        public class Events
        {
            /// <summary>
            /// Gets or sets the @onchange of the field.
            /// </summary>
            public string OnChange { get; set; }

            /// <summary>
            /// Gets or sets the @onblur of the field.
            /// </summary>
            public string OnBlur { get; set; }

            /// <summary>
            /// Gets or sets the @onfocus of the field.
            /// </summary>
            public string OnFocus { get; set; }

            /// <summary>
            /// Gets or sets the @onrender of the field.
            /// </summary>
            public string OnRender { get; set; }
        }

        /// <summary>
        /// Represents the events for a list field.
        /// </summary>
        public class ListEvents : Events
        {
            /// <summary>
            /// Gets or sets the @onclick of the field.
            /// </summary>
            public string OnClick { get; set; }
        }

        /// <summary>
        /// Represents the events for a range field.
        /// </summary>
        public class RangeEvents : Events
        {
            /// <summary>
            /// Gets or sets the @onformat of the field.
            /// </summary>
            public string OnFormat { get; set; }
        }

        /// <summary>
        /// Represents the events used when data-binding an autocomplete field.
        /// </summary>
        public class DataBindingEvents
        {
            /// <summary>
            /// Gets or sets the @onparams of the field.
            /// </summary>
            public string OnParameterSetup { get; set; }

            /// <summary>
            /// Gets or sets the @onrendervalue of the field.
            /// </summary>
            public string OnRenderValue { get; set; }

            /// <summary>
            /// Gets or sets the @onrenderdisplay of the field.
            /// </summary>
            public string OnRenderDisplay { get; set; }
        }
    }
}
