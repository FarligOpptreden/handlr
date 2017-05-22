using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Exposes methods to build translations.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Builds a translation using the specified arguments.
        /// </summary>
        /// <param name="absolutePath">The absolute path to the route module</param>
        /// <param name="relativePath">The relative path to the route module</param>
        /// <param name="configuration">The configuration markup representing the translation</param>
        /// <returns></returns>
        public static ITranslation Build(string absolutePath, string relativePath, XElement configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            string tag = configuration.Name.ToString();
            string tagType = configuration.Attribute("type") != null ? configuration.Attribute("type").Value : null;
            var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where typeof(ITranslation).IsAssignableFrom(type)
                        where type.FullName.ToLower() == tagType.ToLower()
                        select type;

            if (types.FirstOrDefault() == null)
                throw new ArgumentException("A valid configuration could not be loaded for the tag \"" + tag + "\".", "configuration");

            var translationType = types.FirstOrDefault();
            Type loaderType = null;
            var currentType = translationType;
            while (loaderType == null || currentType != typeof(object))
            {
                if (typeof(ITranslation).IsAssignableFrom(currentType) && currentType.GetGenericArguments().Count() == 3)
                {
                    loaderType = currentType.GetGenericArguments()[0];
                }
                currentType = currentType.BaseType;
            }

            if (loaderType == null || translationType == null)
                throw new TargetInvocationException("Invalid ITranslation implementation supplied. Please supply a type that inherits from Translators.Base.", null);

            var translationInstance = Activator.CreateInstance(translationType);
            var loaderArgsInstance = Activator.CreateInstance(loaderType, absolutePath, relativePath, configuration);

            if (translationInstance == null || loaderArgsInstance == null)
                throw new TargetInvocationException("Could not instantiate the ITranslation or ILoaderArguments implementations.", null);

            var loadMethod = translationType.GetMethod("Load", new Type[] { loaderType });
            loadMethod.Invoke(translationInstance, new object[] { loaderArgsInstance });

            return (ITranslation)translationInstance;
        }
    }
}
