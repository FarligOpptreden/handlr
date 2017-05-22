﻿using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Loaders;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Represents a base class for building translation parsers.
    /// </summary>
    public abstract class Parser : Base<TranslationLoaderArguments>
    {
        private string _ListPre;
        private string _ListPost;
        private string _Concat;
        private string _FieldTermination;

        public Parser(string listPre, string listPost, string concat, string fieldTermExpr)
        {
            _ListPre = listPre;
            _ListPost = listPost;
            _Concat = concat;
            _FieldTermination = fieldTermExpr;
        }

        /// <summary>
        /// Loads the template specified in the loader arguments.
        /// </summary>
        /// <returns>The template to apply during the translation</returns>
        protected string LoadTemplate()
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

            return template;
        }

        /// <summary>
        /// Parses a "foreach" statement in the template and applies the template to each object in the list of data.
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="inputData">The field cache containing all keys to apply</param>
        /// <param name="parsedValue">The parsed value</param>
        /// <returns></returns>
        protected bool TryParseForLoop(string value, IFieldCache inputData, out string parsedValue)
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
            string parsedTemplate = _ListPre;

            if (!(dataMember is List<object>))
                throw new ParserException(string.Format("The data member being accessed for the foreach({0} in {1} do [template]) expression is not a valid list.", elementName, dataName));

            foreach (var element in dataMember as List<object>)
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
        /// Parses member accessors in the template.
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="inputData">The field cache containing all keys to apply</param>
        /// <param name="parsedValue">The parsed value</param>
        /// <returns></returns>
        protected bool TryParseDataMembers(string value, IFieldCache inputData, out string parsedValue)
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
                string replaceMatch = fieldMatch.Value.Replace(".", "\\.") + _FieldTermination;
                value = Regex.Replace(value, replaceMatch, elementMember.ToJson(), RegexOptions.IgnoreCase);
            }
            parsedValue = value;
            return true;
        }
    }
}
