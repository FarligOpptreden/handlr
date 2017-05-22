using Handlr.Framework.Web.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Attributes;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// A factory for creating steps.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Builds a step using the supplied configuration markup.
        /// </summary>
        /// <param name="absolutePath">The absolute path to the module being loaded</param>
        /// <param name="relativePath">The relative path to the module being loaded</param>
        /// <param name="configuration">Markup representing the step's configuration</param>
        /// <param name="executionContext">The current execution context</param>
        /// <returns>An initialized step</returns>
        /// <exception cref="ArgumentNullException">Thrown when the configuration parameter is null</exception>
        /// <exception cref="ArgumentException">Throw when the supplied configuration is not valid</exception>
        /// <exception cref="TargetInvocationException">
        /// Thrown when either:
        ///   the loaded IStep implementation does not inherit from the Steps.Base class or
        ///   isntances of the IStep or ILoaderArguments implementations could not be created
        /// </exception>
        public static IStep Build(string absolutePath, string relativePath, XElement configuration, IController executionContext)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            // Find a type in the executing assembly that implements the IStep interface that also has a Tag attribute matching the configuration element's tag name
            string tag = configuration.Name.ToString();
            string tagType = configuration.Attribute("type") != null ? configuration.Attribute("type").Value : null;
            var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where typeof(IStep).IsAssignableFrom(type)
                        let tags = type.GetCustomAttribute<Tag>(true)
                        where tags != null && tags.Tags != null && tags.Tags.Count > 0
                        from t in tags.Tags
                        where t.ToLower() == tag.ToLower() && (string.IsNullOrEmpty(tagType) || type.FullName.ToLower() == tagType.ToLower())
                        select type;

            if (types.FirstOrDefault() == null)
                throw new ArgumentException("A valid configuration could not be loaded for the tag \"" + tag + "\".", "configuration");

            // Get the type of step and the generic types passed as arguments. The generic types are for the loader and field cache types
            var stepType = types.FirstOrDefault();
            Type loaderType = null;
            var currentType = stepType;
            while (loaderType == null || currentType != typeof(object))
            {
                if (typeof(IStep).IsAssignableFrom(currentType) && currentType.GetGenericArguments().Count() == 2)
                {
                    loaderType = currentType.GetGenericArguments()[0];
                    break;
                }
                currentType = currentType.BaseType;
            }
            var fieldCacheType = currentType.GetGenericArguments()[1];

            if (loaderType == null || fieldCacheType == null)
                throw new TargetInvocationException("Invalid IStep implementation supplied. Please supply a type that inherits from Steps.Base<L, C>.", null);

            // Instantiate both the step and the loader arguments
            var stepInstance = Activator.CreateInstance(stepType, executionContext);
            var loaderArgsInstance = Activator.CreateInstance(loaderType, absolutePath, relativePath, configuration);

            if (stepInstance == null || loaderArgsInstance == null)
                throw new TargetInvocationException("Could not instantiate the IStep or ILoaderArguments implementations.", null);

            // Find the "Load" method of the step and pass the loader arguments instance for initialization
            var loadMethod = stepType.GetMethod("Load", new Type[] { loaderType });
            loadMethod.Invoke(stepInstance, new object[] { loaderArgsInstance });

            return (IStep)stepInstance;
        }
    }
}
