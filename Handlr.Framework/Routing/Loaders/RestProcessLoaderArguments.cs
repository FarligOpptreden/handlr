using Handlr.Framework.Routing.Process;
using System.Xml.Linq;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents loader arguments for initializing a REST process.
    /// </summary>
    public class RestProcessLoaderArguments : Base
    {
        /// <summary>
        /// Gets the resource location of the REST process.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the access method for the REST process.
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the content type that is consumed by the REST process, typically defined by the 
        /// "Content-Type" header.
        /// </summary>
        public string Consumes { get; private set; }

        /// <summary>
        /// Gets the content type that is produced by the REST process, typically defined by the 
        /// "Accept" header.
        /// </summary>
        public string Produces { get; private set; }

        /// <summary>
        /// Gets or sets the arguments supplied when initiating the process factory.
        /// </summary>
        public RestProcessFactoryArguments FactoryArgs { get; set; }

        /// <summary>
        /// Creates a new RestProcessLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public RestProcessLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            Url = Configuration.Attribute("url").Value;
            Method = Configuration.Attribute("method").Value;
            Consumes = Configuration.Attribute("consumes") != null ? Configuration.Attribute("consumes").Value : null;
            Produces = Configuration.Attribute("produces") != null ? Configuration.Attribute("produces").Value : null;
        }
    }
}
