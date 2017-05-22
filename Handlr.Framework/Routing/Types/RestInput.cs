using System.Collections.Generic;
using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents the input from an HTTP request using the REST standard.
    /// </summary>
    public class RestInput : IInput
    {
        /// <summary>
        /// Gets the key of the input stream in the global REST field cache.
        /// </summary>
        public const string InputStreamKey = "InputStream";

        /// <summary>
        /// Gets all path variables derived from the request path.
        /// </summary>
        public Dictionary<string, string> PathVariables { get; private set; }

        /// <summary>
        /// Gets all query string parameters passed to the request.
        /// </summary>
        public Dictionary<string, string> QueryParameters { get; private set; }

        /// <summary>
        /// Gets all POST body variables passed to the request.
        /// </summary>
        public Dictionary<string, string> PostVariables { get; private set; }

        /// <summary>
        /// Gets a serialized view of the request body stream.
        /// </summary>
        public string InputStream { get; private set; }

        /// <summary>
        /// Gets the initial REST field cache.
        /// </summary>
        public Dictionary<string, object> FieldCache { get; private set; } = new Dictionary<string, object>();

        public RestInput(Dictionary<string, string> pathVariables, Dictionary<string, string> queryParameters, Dictionary<string, string> postVariables, string inputStream)
        {
            PathVariables = pathVariables;
            QueryParameters = queryParameters;
            PostVariables = postVariables;
            InputStream = inputStream;
            FieldCache
                .AddRange(pathVariables)
                .AddRange(queryParameters)
                .AddRange(postVariables);
            if (!string.IsNullOrEmpty(inputStream))
                FieldCache.Add(InputStreamKey, inputStream);
        }
    }
}
