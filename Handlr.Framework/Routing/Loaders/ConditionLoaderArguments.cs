using System.Xml.Linq;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents the loader arguments for initializing a condition step.
    /// </summary>
    public class ConditionLoaderArguments : Base
    {
        /// <summary>
        /// Gets the test to be applied for the condition.
        /// </summary>
        public string Test { get; private set; }

        /// <summary>
        /// Indicates whether the condition has steps to execute or not.
        /// </summary>
        public bool HasSteps
        {
            get
            {
                return Configuration.HasElements;
            }
        }

        /// <summary>
        /// Creates a new ConditionLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public ConditionLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            Test = configuration.Attribute("test").Value;
        }
    }
}
