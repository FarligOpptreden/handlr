using Handlr.Framework.Web;
using Handlr.Framework.Web.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handlr.Framework.Routing.Process
{
    /// <summary>
    /// Represents the factory arguments for building a REST process.
    /// </summary>
    public class RestProcessFactoryArguments
    {
        /// <summary>
        /// Gets the HTTP handler that is handling the execution context.
        /// </summary>
        public Handler Handler { get; private set; }

        /// <summary>
        /// Gets the path to where module configurations are hosted.
        /// </summary>
        public string ModulePath { get; private set; }

        /// <summary>
        /// Gets the name of the module to construct.
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// Gets the relative path to where the module.
        /// </summary>
        public string Module { get; private set; }

        /// <summary>
        /// Gets the access method of the request.
        /// </summary>
        public Method RequestMethod { get; private set; }

        /// <summary>
        /// Gets the string representation of the access method of the request.
        /// </summary>
        public string MappedRequestMethod { get { return RequestMethod.ToString().ToUpper(); } }

        /// <summary>
        /// Gets the content type being consumed by the request.
        /// </summary>
        public ContentType RequestConsumes { get; private set; }

        /// <summary>
        /// Gets the string representation of the type being consumed by the request.
        /// </summary>
        public string MappedRequestConsumes { get { return GetMappedContentType(RequestConsumes); } }

        /// <summary>
        /// Gets the content type being produced by the request.
        /// </summary>
        public ContentType RequestProduces { get; private set; }

        /// <summary>
        /// Gets the string representation of the type being produced by the request.
        /// </summary>
        public string MappedRequestProduces { get { return GetMappedContentType(RequestProduces); } }

        /// <summary>
        /// Gets the module segments of the request url.
        /// </summary>
        public string[] RequestUrlSegments { get; private set; }

        /// <summary>
        /// Gets the string representation of the module segments of the request url.
        /// </summary>
        public string MappedRequestUrlSegments { get { return RequestUrlSegments.Flatten("-").ToLower(); } }

        /// <summary>
        /// Gets the path variables in the request url.
        /// </summary>
        public Dictionary<string, string> RequestPathVariables { get { return Handler.PathVariables; } }

        /// <summary>
        /// Gets the query parameters in the request url.
        /// </summary>
        public Dictionary<string, string> RequestQueryParameters { get { return Handler.Parameters; } }

        /// <summary>
        /// Gets the form post variables in the request body.
        /// </summary>
        public Dictionary<string, string> RequestPostVariables { get { return Handler.Post; } }

        /// <summary>
        /// Gets the serialized input stream for the request.
        /// </summary>
        public string RequestInputStream { get { return Handler.InputStream; } }

        /// <summary>
        /// Creates a new RestProcessFactoryArguments instance.
        /// </summary>
        /// <param name="handler">The HTTP handler for the context request</param>
        /// <param name="modulePath">The path to where module configurations are hosted</param>
        /// <param name="moduleName">The name of the module to construct</param>
        /// <exception cref="ArgumentNullException">Thrown when the handler parameter is null</exception>
        public RestProcessFactoryArguments(Handler handler, string modulePath, string moduleName)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            ModulePath = handler.Server.MapPath(modulePath);
            ModuleName = moduleName;
            Module = "~" + modulePath + "/" + moduleName;
            int index = 0;
            RequestUrlSegments =
                (
                    from segment in handler.UriSegments
                    where index++ > 1
                    select segment
                ).ToArray();
            RequestMethod = handler.RequestMethod;
            RequestConsumes = handler.RequestType;
            RequestProduces = handler.ResponseType;
            Handler = handler;
        }

        /// <summary>
        /// Gets a string representation of the supplied content type.
        /// </summary>
        /// <param name="type">The content type to define a string representation of</param>
        /// <returns>A string representation of the content type</returns>
        public static string GetMappedContentType(ContentType type)
        {
            switch (type)
            {
                case ContentType.Json:
                case ContentType.JavaScript:
                    return "JSON";
                case ContentType.Html:
                case ContentType.Form:
                    return "HTML";
                case ContentType.Xml:
                case ContentType.SOAP:
                    return "XML";
                default:
                    return "OTHER";
            }
        }
    }
}
