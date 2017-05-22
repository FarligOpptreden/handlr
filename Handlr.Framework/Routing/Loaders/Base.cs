using System;
using System.Xml.Linq;
using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Provides a base for all ILoaderArguments implementations. This class cannot be 
    /// instantiated.
    /// </summary>
    public abstract class Base : ILoaderArguments
    {
        /// <summary>
        /// Gets the configuration element of the loader arguments.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throw when the configuration parameter is null</exception>
        public XElement Configuration { get; private set; }

        /// <summary>
        /// Gets the relative path to the module that contains the loader arguments.
        /// </summary>
        public string RelativePath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the module that contains the loader arguments.
        /// </summary>
        public string AbsolutePath { get; private set; }

        /// <summary>
        /// The base constructor for all sub classes.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        /// <exception cref="ArgumentNullException">Thrown when the modulePath or configuration parameters are null</exception>
        public Base(string absolutePath, string relativePath, XElement configuration)
        {
            if (absolutePath == null)
                throw new ArgumentNullException("modulePath");
            if (relativePath == null)
                throw new ArgumentNullException("module");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            AbsolutePath = absolutePath;
            RelativePath = relativePath;
            Configuration = configuration;
        }
    }
}
