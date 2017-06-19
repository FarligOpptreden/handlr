using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Types;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Represents a base class for building JSON readers or writers.
    /// </summary>
    public abstract class JsonParser : Parser
    {
        public JsonParser() : base ("[", "]", ",", "(?=,|\\s|\\r|\\n|}|\\))") { }

        /// <summary>
        /// Loads the translation file at the configured location and parses the supplied input according to the template.
        /// </summary>
        /// <param name="fieldCache">The field cache to parse in the template</param>
        /// <returns>A generic field cache representing the parsed template</returns>
        protected IFieldCache LoadAndParseTemplate(IFieldCache fieldCache)
        {
            string template = LoadTemplate();
            
            while (TryParseForLoop(template, fieldCache, out template)) { }
            while (TryParseDataMembers(template, fieldCache, out template)) { }

            GenericFieldCache parsed = null;
            try
            {
                parsed = new GenericFieldCache(template.ToDictionary());
            }
            catch { }

            if (string.IsNullOrEmpty(template) || parsed == null)
                throw new ParserException("The JSON translation could not be applied to the source data.");

            return parsed;
        }

        /// <summary>
        /// Overridden implementation of the abstract method
        /// </summary>
        /// <returns></returns>
        protected override bool TryParseForLoop(string value, IFieldCache inputData, out string parsedValue)
        {
            var match = Regex.Match(value, "foreach\\s?\\((@[a-zA-Z0-9]+)\\s+in\\s+(@[a-zA-Z0-9\\.]+)\\s+do\\s+([a-zA-Z0-9!@#%^&*(),{}\"':_=+\\.\\-\\[\\]\\r\\n\\s\\t]*)\\)\\$", RegexOptions.IgnoreCase);
            if (match == null || !match.Success)
            {
                parsedValue = value;
                return false;
            }

            if (match.Groups.Count != 4)
                throw new ParserException(string.Format("The foreach() expression is not formatted properly. Make sure it is in the format foreach(@element in @data do [template body])."));

            string elementName = match.Groups[1].Value;
            string dataName = match.Groups[2].Value.Substring(1);
            object dataMember = Utilities.GetDataMember(dataName.Split('.'), inputData);
            string template = match.Groups[3].Value;
            string parsedTemplate = _ListPre;

            if (!typeof(IEnumerable).IsAssignableFrom(dataMember.GetType()))
                throw new ParserException(string.Format("The data member being accessed for the foreach({0} in {1} do [template]) expression is not a valid list.", elementName, dataName));

            foreach (var element in dataMember as IEnumerable)
            {
                string elementTemplate = template;
                if (element is Dictionary<string, object>)
                    while (TryParseForLoop(elementTemplate, dataMember as IFieldCache, out elementTemplate)) { }
                foreach (Match fieldMatch in Regex.Matches(elementTemplate, elementName + "(\\.[a-zA-Z0-9\\-_]+)*", RegexOptions.IgnoreCase))
                {
                    object elementMember = Utilities.GetDataMember(fieldMatch.Value.Split('.'), element);
                    string replaceMatch = fieldMatch.Value.Replace(".", "\\.") + _FieldTermination;
                    elementTemplate = Regex.Replace(elementTemplate, replaceMatch, elementMember.ToJson(), RegexOptions.IgnoreCase);
                }
                parsedTemplate += elementTemplate + _Concat;
            }

            if (parsedTemplate.Length > 1)
                parsedTemplate = parsedTemplate.Substring(0, parsedTemplate.Length - 1);
            parsedTemplate += _ListPost;
            parsedValue = value.Replace(match.Value, parsedTemplate);
            return true;
        }

        /// <summary>
        /// Overridden implementation of the abstract method
        /// </summary>
        /// <returns></returns>
        protected override bool TryParseDataMembers(string value, IFieldCache inputData, out string parsedValue)
        {
            var matches = Regex.Matches(value, "[\\s\\t\\n]+@([a-zA-Z0-9\\-_\\[\\]]+\\.?)*", RegexOptions.IgnoreCase);
            if (matches == null || matches.Count == 0)
            {
                parsedValue = value;
                return false;
            }
            foreach (Match fieldMatch in matches)
            {
                object elementMember = null;
                if (fieldMatch.Value.Trim().ToLower() == "@input")
                    elementMember = inputData;
                else
                    elementMember = Utilities.GetDataMember(fieldMatch.Value.Trim().Substring(1).Split('.'), inputData);
                string replaceMatch = fieldMatch.Value.Replace(".", "\\.").Replace("]", "\\]").Replace("[", "\\[") + _FieldTermination;
                value = Regex.Replace(value, replaceMatch, elementMember.ToJson(), RegexOptions.IgnoreCase);
            }
            parsedValue = value;
            return true;
        }
    }
}
