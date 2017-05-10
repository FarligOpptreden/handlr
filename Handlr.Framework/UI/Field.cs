﻿using System.Linq;
using System.Text;
using System.Web;

namespace Handlr.Framework.UI
{
    /// <summary>
    /// Exposes methods to render the various input fields.
    /// </summary>
    public static partial class Fields
    {
        /// <summary>
        /// Renders a regular text field.
        /// </summary>
        /// <param name="args">The arguments used to specify all the field attributes</param>
        /// <returns>A formatted representation of a text field</returns>
        public static IHtmlString Text(TextArguments args)
        {
            return new HtmlString(string.Format(
                "<h-field type=\"text\" id=\"{0}\" label=\"{1}\" value=\"{2}\" placeholder=\"{3}\" {4} {5} {6} class=\"{7}\" maxlength=\"{8}\" onchange=\"{9}\" onblur=\"{10}\" onfocus=\"{11}\" onrender=\"{12}\" />",
                args.Id,
                args.Label,
                args.Value,
                args.Placeholder,
                args.Multiline ? "multiline" : "",
                args.Readonly ? "readonly" : "",
                args.Disabled ? "disabled" : "",
                args.Classes,
                args.MaxLength.HasValue ? args.MaxLength.Value.ToString() : null,
                args.Events != null ? args.Events.OnChange : null,
                args.Events != null ? args.Events.OnBlur : null,
                args.Events != null ? args.Events.OnFocus : null,
                args.Events != null ? args.Events.OnRender : null
                ));
        }

        /// <summary>
        /// Renders a numeric text field.
        /// </summary>
        /// <param name="args">The arguments used to specify all the field attributes</param>
        /// <returns>A formatted representation of a number field</returns>
        public static IHtmlString Number(NumberArguments args)
        {
            return new HtmlString(string.Format(
                "<h-field type=\"number\" id=\"{0}\" label=\"{1}\" value=\"{2}\" placeholder=\"{3}\" {4} {5} class=\"{6}\" onchange=\"{7}\" onblur=\"{8}\" onfocus=\"{9}\" onrender=\"{10}\" />",
                args.Id,
                args.Label,
                args.Value,
                args.Placeholder,
                args.Readonly ? "readonly" : "",
                args.Disabled ? "disabled" : "",
                args.Classes,
                args.Events != null ? args.Events.OnChange : null,
                args.Events != null ? args.Events.OnBlur : null,
                args.Events != null ? args.Events.OnFocus : null,
                args.Events != null ? args.Events.OnRender : null
                ));
        }

        /// <summary>
        /// Renders a checkbox field.
        /// </summary>
        /// <param name="args">The arguments used to specify all the field attributes</param>
        /// <returns>A formatted representation of a number field</returns>
        public static IHtmlString Checkbox(CheckboxArguments args)
        {
            return new HtmlString(string.Format(
                "<h-field type=\"checkbox\" id=\"{0}\" label=\"{1}\" value=\"{2}\" {3} {4} class=\"{5}\" onchange=\"{6}\" onblur=\"{7}\" onfocus=\"{8}\" onrender=\"{9}\" />",
                args.Id,
                args.Label,
                args.Value,
                args.Readonly ? "readonly" : "",
                args.Disabled ? "disabled" : "",
                args.Classes,
                args.Events != null ? args.Events.OnChange : null,
                args.Events != null ? args.Events.OnBlur : null,
                args.Events != null ? args.Events.OnFocus : null,
                args.Events != null ? args.Events.OnRender : null
                ));
        }

        /// <summary>
        /// Renders a calendar field.
        /// </summary>
        /// <param name="args">The arguments used to specify all the field attributes</param>
        /// <returns>A formatted representation of a number field</returns>
        public static IHtmlString Calendar(CalendarArguments args)
        {
            return new HtmlString(string.Format(
                "<h-field type=\"calendar\" id=\"{0}\" label=\"{1}\" value=\"{2}\" {3} {4} {5} format=\"{6}\" class=\"{7}\" onchange=\"{8}\" onblur=\"{9}\" onfocus=\"{10}\" onrender=\"{11}\" />",
                args.Id,
                args.Label,
                args.Value,
                args.Readonly ? "readonly" : "",
                args.Disabled ? "disabled" : "",
                args.Time ? "time" : "",
                args.Format,
                args.Classes,
                args.Events != null ? args.Events.OnChange : null,
                args.Events != null ? args.Events.OnBlur : null,
                args.Events != null ? args.Events.OnFocus : null,
                args.Events != null ? args.Events.OnRender : null
                ));
        }

        /// <summary>
        /// Renders a list field.
        /// </summary>
        /// <param name="args">The arguments used to specify all the field attributes</param>
        /// <returns>A formatted representation of a number field</returns>
        public static IHtmlString List(ListArguments args)
        {
            StringBuilder items = new StringBuilder();
            if (args.Items != null && args.Items.Count > 0)
                items.Append("<items>");
            foreach (var item in args.Items)
                items.Append(string.Format("<item value=\"{0}\" display=\"{1}\" />", item.Value, item.Display));
            if (args.Items != null && args.Items.Count > 0)
                items.Append("</items>");

            return new HtmlString(string.Format(
                "<h-field type=\"list\" id=\"{0}\" label=\"{1}\" value=\"{2}\" {3} {4} class=\"{5}\" onchange=\"{6}\" onblur=\"{7}\" onfocus=\"{8}\" onrender=\"{9}\">{10}</h-field>",
                args.Id,
                args.Label,
                args.Value,
                args.Readonly ? "readonly" : "",
                args.Disabled ? "disabled" : "",
                args.Classes,
                args.Events != null ? args.Events.OnChange : null,
                args.Events != null ? args.Events.OnBlur : null,
                args.Events != null ? args.Events.OnFocus : null,
                args.Events != null ? args.Events.OnRender : null,
                items.ToString()
                ));
        }

        /// <summary>
        /// Renders an autocomplete field.
        /// </summary>
        /// <param name="args">The arguments used to specify all the field attributes</param>
        /// <returns>A formatted representation of a number field</returns>
        public static IHtmlString Autocomplete(AutocompleteArguments args)
        {
            string valueBinding = "";
            if (args.ValueBinding != null)
                valueBinding = string.Format("<value value=\"{0}\" display=\"{1}\" context=\"{2}\" />", args.ValueBinding.Value, args.ValueBinding.Display, args.ValueBinding.Context);

            string dataBinding = "";
            if (args.DataBinding != null)
                dataBinding = string.Format("<autocomplete url=\"{0}\" method=\"{1}\" parameters=\"{2}\" onparams=\"{3}\" onrendervalue=\"{4}\" onrenderdisplay=\"{5}\" fieldaccessor=\"{6}\" />",
                    args.DataBinding.Url,
                    args.DataBinding.Method.ToString().ToUpper(),
                    "{ " + args.DataBinding.Parameters.Select(p => "'" + p.Key + "': '" + p.Value + "'").Flatten(",") + " }",
                    args.DataBinding.Events != null ? args.DataBinding.Events.OnParameterSetup : null,
                    args.DataBinding.Events != null ? args.DataBinding.Events.OnRenderValue : null,
                    args.DataBinding.Events != null ? args.DataBinding.Events.OnRenderDisplay : null,
                    args.DataBinding.FieldAccessor
                    );

            return new HtmlString(string.Format(
                "<h-field type=\"list\" id=\"{0}\" label=\"{1}\" value=\"{2}\" {3} {4} class=\"{5}\" onchange=\"{6}\" onblur=\"{7}\" onfocus=\"{8}\" onrender=\"{9}\">{10}{11}</h-field>",
                args.Id,
                args.Label,
                args.Value,
                args.Readonly ? "readonly" : "",
                args.Disabled ? "disabled" : "",
                args.Classes,
                args.Events != null ? args.Events.OnChange : null,
                args.Events != null ? args.Events.OnBlur : null,
                args.Events != null ? args.Events.OnFocus : null,
                args.Events != null ? args.Events.OnRender : null,
                valueBinding,
                dataBinding
                ));
        }
    }
}
