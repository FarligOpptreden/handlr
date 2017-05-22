using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Loaders;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Represents a base class for building POST body readers or writers.
    /// </summary>
    /// <typeparam name="I">The type of input object to consume</typeparam>
    /// <typeparam name="O">The type of output object to produce</typeparam>
    public abstract class PostParser<I, O> : Base<TranslationLoaderArguments, I, O>
        where I : IInput
        where O : IOutput
    {
        /// <summary>
        /// Loads the translation file at the configured location and parses the supplied input according to the template.
        /// </summary>
        /// <param name="fieldCache">The field cache to parse in the template</param>
        /// <returns>A POST field cache representing the parsed template</returns>
        protected PostFieldCache LoadAndParseTemplate(Dictionary<string, object> fieldCache)
        {
            string path = LoaderArguments.TemplatePath.Replace("/", "\\").Replace("{AbsolutePath}", LoaderArguments.AbsolutePath);
            FileInfo fi = new FileInfo(path);

            if (!fi.Exists)
                throw new ParserException(string.Format("The translation file \"{0}\" could not be loaded as the file does not exist", path));

            string template = null;
            using (StreamReader reader = new StreamReader(path))
            {
                template = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(template))
                throw new ParserException(string.Format("The translation file \"{0}\" is empty", path));

            template = template.Replace("\r", "").Replace("\n", "");

            while (TryParseForLoop(template, fieldCache, out template)) { }
            while (TryParseDataMembers(template, fieldCache, out template)) { }

            PostFieldCache parsed = null;
            try
            {
                parsed = new PostFieldCache(template);
            }
            catch { }

            if (string.IsNullOrEmpty(template) || parsed == null)
                throw new ParserException("The translation could not be applied to the source data.");

            return parsed;
        }

        /// <summary>
        /// Parses a "foreach" statement in the template and applies the template to each object in the list of data.
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="inputData">The field cache containing all keys to apply</param>
        /// <param name="parsedValue">The parsed value</param>
        /// <returns></returns>
        protected bool TryParseForLoop(string value, Dictionary<string, object> inputData, out string parsedValue)
        {
            var match = Regex.Match(value, "foreach\\s?\\((@[a-zA-Z0-9]+)\\s+in\\s+(@[a-zA-Z0-9\\.]+)\\s+do\\s+([[a-zA-Z0-9\\s!@#$%^&*(),{}\"':_=+\\.\\-\\[\\]\r\\n\\s\\t]*)\\)", RegexOptions.IgnoreCase);
            if (match == null || !match.Success)
            {
                parsedValue = value;
                return false;
            }

            if (match.Groups.Count != 4)
                throw new ParserException(string.Format("The foreach() expression is not formatted properly. Make sure it is in the format foreach(@element in @data do [template body])."));

            string elementName = match.Groups[1].Value;
            string dataName = match.Groups[2].Value;
            object dataMember = Utilities.GetDataMember(dataName.Split('.'), inputData);
            string template = match.Groups[3].Value;
            string parsedTemplate = "";

            if (!(dataMember is List<object>))
                throw new ParserException(string.Format("The data member being accessed for the foreach({0} in {1} do [template]) expression is not a valid list.", elementName, dataName));

            foreach (var element in dataMember as List<object>)
            {
                string elementTemplate = template;
                if (element is Dictionary<string, object>)
                    while (TryParseForLoop(elementTemplate, dataMember as Dictionary<string, object>, out elementTemplate)) { }
                foreach (Match fieldMatch in Regex.Matches(elementTemplate, elementName + "(\\.[a-zA-Z0-9\\-_]+)*", RegexOptions.IgnoreCase))
                {
                    object elementMember = Utilities.GetDataMember(fieldMatch.Value.Split('.'), element);
                    string replaceMatch = fieldMatch.Value.Replace(".", "\\.") + "(?=\\=|\\&|\\s|\\r|\\n|\\)|$)";
                    elementTemplate = Regex.Replace(elementTemplate, replaceMatch, elementMember.ToString(), RegexOptions.IgnoreCase);
                }
                parsedTemplate += elementTemplate + "&";
            }

            if (parsedTemplate.Length > 1)
                parsedTemplate = parsedTemplate.Substring(0, parsedTemplate.Length - 1);
            parsedValue = value.Replace(match.Value, parsedTemplate);
            return true;
        }

        /// <summary>
        /// Parses member accessors in the template.
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="inputData">The field cache containing all keys to apply</param>
        /// <param name="parsedValue">The parsed value</param>
        /// <returns></returns>
        protected bool TryParseDataMembers(string value, Dictionary<string, object> inputData, out string parsedValue)
        {
            var matches = Regex.Matches(value, "@input(\\.[a-zA-Z0-9\\-_]+)*", RegexOptions.IgnoreCase);
            if (matches == null || matches.Count == 0)
            {
                parsedValue = value;
                return false;
            }
            foreach (Match fieldMatch in matches)
            {
                object elementMember = Utilities.GetDataMember(fieldMatch.Value.Split('.'), inputData);
                string replaceMatch = fieldMatch.Value.Replace(".", "\\.") + "(?=\\=|\\&|\\s|\\r|\\n|\\)|$)";
                value = Regex.Replace(value, replaceMatch, elementMember.ToString(), RegexOptions.IgnoreCase);
            }
            parsedValue = value;
            return true;
        }
    }
}
