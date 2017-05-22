﻿using Handlr.Framework.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Exceptions;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Provides a base for building IStep implementations. This class cannot be instantiated.
    /// </summary>
    /// <typeparam name="L">The type of loader arguments to use</typeparam>
    /// <typeparam name="C">The type of field cache to use</typeparam>
    public abstract class Base<L, C> : IStep
        where L : ILoaderArguments
        where C : IFieldCache
    {
        private C FieldCache { get; set; }

        /// <summary>
        /// Gets the current execution context of the step.
        /// </summary>
        public IController ExecutionContext { get; private set; }

        /// <summary>
        /// Gets the loader arguments for the step.
        /// </summary>
        public L LoaderArguments { get; private set; }

        /// <summary>
        /// Creates a new Base instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public Base(IController executionContext)
        {
            ExecutionContext = executionContext;
        }

        /// <summary>
        /// Executes the step using the supplied field cache as input data.
        /// </summary>
        /// <param name="fieldCache">The field cache to use as input data</param>
        /// <returns>The updated field cache</returns>
        public abstract C ExecuteStep(C fieldCache);

        /// <summary>
        /// Executes the step using the supplied field cache as input data.
        /// </summary>
        /// <typeparam name="T">The type of field cache to use</typeparam>
        /// <param name="fieldCache">The field cache to use as input data</param>
        /// <returns>The updated field cache</returns>
        /// <exception cref="ArgumentNullException">Throw when the fieldCache parameter is null</exception>
        public T Execute<T>(T fieldCache) where T : IFieldCache
        {
            if (fieldCache == null)
                throw new ArgumentNullException("fieldCache");

            FieldCache = (C)(IFieldCache)fieldCache;
            return (T)(IFieldCache)ExecuteStep((C)(IFieldCache)fieldCache);
        }

        /// <summary>
        /// Initializes the step using the supplied loader arguments.
        /// </summary>
        /// <param name="loaderArgs">The loader arguments to initialize with</param>
        /// <exception cref="ArgumentNullException">Thrown when the fieldCache parameter is null</exception>
        public virtual void Load(L loaderArgs)
        {
            if (loaderArgs == null)
                throw new ArgumentNullException("loaderArgs");

            LoaderArguments = loaderArgs;
        }

        /// <summary>
        /// Initializes the step using the supplied loader arguments.
        /// </summary>
        /// <typeparam name="T">The type of loader arguments to use</typeparam>
        /// <param name="loaderArgs">The loader arguments to initialize with</param>
        public void Load<T>(T loaderArgs) where T : ILoaderArguments
        {
            Load(loaderArgs);
        }

        /// <summary>
        /// Parse a value of a config element for special structs.
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <returns>The parsed value with all structs replaced with their evaluated values</returns>
        protected string ParseValue(string value)
        {
            foreach (Match match in Regex.Matches(value, "{.[^{}]+}", RegexOptions.IgnoreCase))
            {
                string matchValue = match.Value;
                while (TryParseConfigBlock(matchValue, out matchValue)) { }
                value = value.Replace(match.Value, matchValue);
                while (TryParseDecryptBlock(matchValue, out matchValue)) { }
                value = value.Replace(match.Value, matchValue);
                while (TryParseFieldValue(matchValue, out matchValue)) { }
                value = value.Replace(match.Value, matchValue);
            }
            value = value.Replace("{", "").Replace("}", "");
            return value;
        }

        /// <summary>
        /// Try and find a "Decrypt" code block in the specified string and parse it if possible.
        /// </summary>
        /// <param name="value">The value to test for the code block</param>
        /// <param name="parsedValue">The parsed value with the evaluated code block's value</param>
        /// <returns>A value indicating whether the code block could be found or not</returns>
        protected bool TryParseDecryptBlock(string value, out string parsedValue)
        {
            var match = Regex.Match(value, "Decrypt(.+)", RegexOptions.IgnoreCase);
            if (match == null || !match.Success)
            {
                parsedValue = value;
                return false;
            }
            try
            {
                string partToDecrypt = Utilities.DecryptString(match.Value.Substring(7, match.Value.Length - 9));
                parsedValue = value.Replace(match.Value, partToDecrypt);
            }
            catch (Exception ex)
            {
                throw new ParserException(string.Format("The value could not be decrypted: {0}", ex.Message));
            }
            return true;
        }

        /// <summary>
        /// Try and find a "Config" code block in the specified string and parse it if possible.
        /// </summary>
        /// <param name="value">The value to test for the code block</param>
        /// <param name="parsedValue">The parsed value with the evaluated code block's value</param>
        /// <returns>A value indicating whether the code block could be found or not</returns>
        protected bool TryParseConfigBlock(string value, out string parsedValue)
        {
            var match = Regex.Match(value, "Config\\.([a-zA-Z0-9\\[\\]\\.\\/\\-_](\\(\\))?)+", RegexOptions.IgnoreCase);
            if (match == null || !match.Success)
            {
                parsedValue = value;
                return false;
            }
            var prop = match.Value.Substring(match.Value.IndexOf(".") + 1);
            var accessor = match.Value.Substring(match.Value.IndexOf("[") + 1);
            accessor = accessor.Substring(0, accessor.Length - 1);
            prop = prop.Substring(0, prop.IndexOf("["));
            switch (prop)
            {
                case "ConnectionStrings":
                    try
                    {
                        parsedValue = value.Replace(match.Value, ExecutionContext.Configuration().ConnectionStrings[accessor]);
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new ParserException(string.Format("The connection string key \"{0}\" does not exist in the Handlr configuration file.", accessor));
                    }
                    break;
                case "Custom":
                    string customValue = ExecutionContext.Configuration().Custom[accessor];
                    if (string.IsNullOrEmpty(customValue))
                        throw new ParserException(string.Format("The custom configuration path \"0\" does not exist in the Handlr configuration file.", accessor));
                    parsedValue = value.Replace(match.Value, customValue);
                    break;
                default:
                    throw new ParserException(string.Format("The configuration property \"{0}\" could not be parsed.", prop));
            }
            return true;
        }

        /// <summary>
        /// Try and find a boolean test code block in the specified string and parse it if possible.
        /// </summary>
        /// <param name="value">The value to test for the code block</param>
        /// <param name="parsedValue">The parsed value with the evaluated code block's value</param>
        /// <returns>A value indicating whether the code block could be found or not</returns>
        protected bool TryParseBooleanBlock(string value, out string parsedValue)
        {
            const string fieldRegex = "{([a-zA-Z0-9\\-_\\.]+)}";
            const string numberRegex = "\\d+(.\\d+)?";
            const string booleanRegex = "true|false";
            const string stringRegex = "'(.*)'";
            const string paramRegex = "(" + fieldRegex + "|" + numberRegex + "|" + booleanRegex + "|" + stringRegex + ")"; // TODO: Fix param regex

            var oneParamMatch = Regex.Match(value, "(Exists)\\(" + paramRegex + "\\)", RegexOptions.IgnoreCase);
            var twoParamMatch = Regex.Match(value, "(Equals|GreaterThan|LowerThan)\\(" + paramRegex + ",\\s?" + paramRegex + "\\)", RegexOptions.IgnoreCase);
            if ((oneParamMatch == null || !oneParamMatch.Success) && (twoParamMatch == null || !twoParamMatch.Success))
            {
                parsedValue = value;
                return false;
            }
            parsedValue = value;
            if (oneParamMatch != null && oneParamMatch.Success)
            {
                string operation = oneParamMatch.Value.Substring(0, oneParamMatch.Value.IndexOf("("));
                string param = oneParamMatch.Groups[3].Value;
                switch (operation.ToUpper())
                {
                    case "EXISTS":
                        parsedValue = value.Replace(oneParamMatch.Value, FieldCache.Exists(param).ToString().ToLower());
                        return true;
                    default:
                        throw new ParserException(string.Format("The boolean operation \"{0}({1})\" could not be parsed.", operation, param));
                }
            }
            if (twoParamMatch != null && twoParamMatch.Success)
            {
                string operation = twoParamMatch.Value.Substring(0, twoParamMatch.Value.IndexOf("("));
                List<string> values = new List<string>();
                foreach (Match match in Regex.Matches(twoParamMatch.Value, paramRegex, RegexOptions.IgnoreCase))
                {
                    if (Regex.IsMatch(match.Value, fieldRegex, RegexOptions.IgnoreCase))
                    {
                        string name = match.Groups[2].Value;
                        if (FieldCache.Exists(name))
                        {
                            var fieldValue = FieldCache.GetValue<object>(name);
                            if (fieldValue.IsPrimitive())
                                fieldValue = fieldValue.ToString().ToLower();
                            else
                                fieldValue = fieldValue.ToJson();
                            values.Add(fieldValue as string);
                        }
                        else
                            values.Add("null");
                        continue;
                    }
                    if (Regex.IsMatch(match.Value, stringRegex, RegexOptions.IgnoreCase))
                    {
                        values.Add(match.Groups[4].Value);
                        continue;
                    }
                    values.Add(match.Value.ToLower());
                }
                switch (operation.ToUpper())
                {
                    case "EQUALS":
                        parsedValue = value.Replace(twoParamMatch.Value, (values[0] == values[1]).ToString().ToLower());
                        break;
                    case "GREATERTHAN":
                        // TODO: Test whether the first item in values is greater than the last item based on value types
                        parsedValue = value.Replace(twoParamMatch.Value, "true");
                        break;
                    case "LOWERTHAN":
                        // TODO: Test whether the first item in values is lower than the last item based on value types
                        parsedValue = value.Replace(twoParamMatch.Value, "true");
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Try and find a {field-accessor} code block in the specified string and parse it if possible.
        /// </summary>
        /// <param name="value">The value to test for the code block</param>
        /// <param name="parsedValue">The parsed value with the evaluated code block's value</param>
        /// <returns>A value indicating whether the code block could be found or not</returns>
        protected bool TryParseFieldValue(string value, out string parsedValue)
        {
            if (string.IsNullOrEmpty(value) || !Regex.IsMatch(value, "\\{.+\\}"))
            {
                parsedValue = value;
                return false;
            }
            string valueToCheck = value.Substring(1, value.Length - 2);
            if (!FieldCache.Exists(valueToCheck))
            {
                parsedValue = "";
                return false;
            }
            parsedValue = FieldCache.GetValue<object>(valueToCheck).ToString();
            return true;
        }
    }
}
