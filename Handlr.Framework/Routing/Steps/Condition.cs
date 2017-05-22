using System;
using System.Collections.Generic;
using Handlr.Framework.Web.Interfaces;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Routing.Loaders;
using System.Text.RegularExpressions;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that executes a condition to determine whether its steps should be
    /// executed or not.
    /// </summary>
    [Tag("Condition")]
    public class Condition : Base<ConditionLoaderArguments, RestFieldCache>
    {
        /// <summary>
        /// Gets the steps that should be executed if the condition succeeds.
        /// </summary>
        public List<IStep> Steps { get; private set; } = new List<IStep>();

        /// <summary>
        /// Creates a new Condition instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public Condition(IController executionContext) : base(executionContext) { }

        /// <summary>
        /// Performs the conditional check. Upon succeeding, the child steps are loaded and
        /// executed in order.
        /// </summary>
        /// <param name="fieldCache">The field cache to use during the conditional check</param>
        /// <returns>An updated field cache containing data returned by all child steps</returns>
        /// <exception cref="ArgumentNullException">Thrown when the fieldCache parameter is null</exception>
        public override RestFieldCache ExecuteStep(RestFieldCache fieldCache)
        {
            if (fieldCache == null)
                throw new ArgumentNullException("fieldCache");

            if (PerformTest(fieldCache) && LoaderArguments.HasSteps)
            {
                // Load the steps configured in the Condition element
                foreach (var step in LoaderArguments.Configuration.Elements())
                    Steps.Add(Factory.Build(LoaderArguments.AbsolutePath, LoaderArguments.RelativePath, step, ExecutionContext));
                // Execute each step and update the field cache according to what the step returns
                foreach (IStep step in Steps)
                    fieldCache = step.Execute(fieldCache);
            }
            return fieldCache;
        }

        /// <summary>
        /// Executes the conditional check.
        /// </summary>
        /// <param name="fieldCache">The field cache to use during the conditional check</param>
        /// <returns>A value indicating whether the conditional check succeeded or not</returns>
        public bool PerformTest(RestFieldCache fieldCache)
        {
            string test = LoaderArguments.Test;
            while (TryParseBooleanBlock(test, out test)) { }
            bool valid = true;
            test = string.Format("({0})", test);
            while (ExecuteBooleanBlock(test, valid, out test, out valid)) { }
            return valid;
        }

        private bool ExecuteBooleanBlock(string test, bool result, out string parsedTest, out bool evaluatedResult)
        {
            var matches = Regex.Matches(test, "\\((true|false|and|or|\\s)+\\)", RegexOptions.IgnoreCase);
            if (matches == null || matches.Count == 0)
            {
                evaluatedResult = result;
                parsedTest = test;
                return false;
            }
            evaluatedResult = result;
            foreach (Match match in matches)
            {
                bool matchResult = true;
                string toTest = match.Groups[3].Value;
                string[] parts = match.Value.Substring(1, match.Value.Length - 2).Split(' ');
                string currentOperand = "and";
                foreach (string part in parts)
                {
                    string trimmed = part.Trim().ToLower();
                    if (trimmed == "and")
                    {
                        currentOperand = "and";
                        continue;
                    }
                    if (trimmed == "or")
                    {
                        currentOperand = "or";
                        continue;
                    }
                    bool parsed = false;
                    if (bool.TryParse(trimmed, out parsed))
                    {
                        matchResult = currentOperand == "and" ? matchResult && parsed : matchResult || parsed;
                        continue;
                    }
                    throw new ParserException(string.Format("The conditional statement \"{0}\" could not be evaluated as a boolean.", test));
                }
                test = test.Replace(match.Value, matchResult.ToString().ToLower());
                evaluatedResult = matchResult;
            }
            parsedTest = test;
            return true;
        }
    }
}
