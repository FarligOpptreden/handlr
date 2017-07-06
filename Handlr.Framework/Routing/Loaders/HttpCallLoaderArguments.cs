using Handlr.Framework.Web.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents the loader arguments for initializing an HTTP route.
    /// </summary>
    public class HttpCallLoaderArguments : Base
    {
        /// <summary>
        /// Gets the translation to be applied before performing the call to the HTTP resource.
        /// </summary>
        public ITranslation InputTranslation { get; private set; }

        /// <summary>
        /// Gets the location of the HTTP resource.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the access method to use when performing the call to the HTTP resource.
        /// </summary>
        public Method Method { get; private set; }

        /// <summary>
        /// Gets the content type to be specified as Accept header when performing the call to the 
        /// HTTP resource.
        /// </summary>
        public ContentType ContentType { get; private set; }

        /// <summary>
        /// Gets any custom headers to be applied when performing the call to the HTTP resource.
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the translation to be applied after retrieving data from the HTTP resource.
        /// </summary>
        public ITranslation OutputTranslation { get; private set; }

        /// <summary>
        /// Gets or sets the attribute value of the Cache property.
        /// </summary>
        public string CacheKey { get; private set; }

        /// <summary>
        /// Gets or set the attribute value of the Cache property.
        /// </summary>
        public string CacheType { get; private set; }

        /// <summary>
        /// Gets the amount of time, in milliseconds, after which the call will time-out.
        /// </summary>
        public int CommandTimeout { get; private set; }

        /// <summary>
        /// Creates a new HttpRouteLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public HttpCallLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            var inputTranslationElement = configuration.XPathSelectElement("./InputTranslation");
            if (inputTranslationElement != null)
                InputTranslation = Translators.Factory.Build(absolutePath, relativePath, inputTranslationElement);
            Url = configuration.XPathEvaluate("string(./Url/text())") as string;
            string method = configuration.XPathEvaluate("string(./Method/text())") as string;
            Method = (Method)Enum.Parse(typeof(Method), method.ToLower().Substring(0, 1).ToUpper() + method.ToLower().Substring(1));
            ContentType = AllTypes.ContentTypeFromStrings((configuration.XPathEvaluate("string(./ContentType/text())") as string).ToLower());
            var headers = configuration.XPathSelectElement("./Headers");
            if (headers != null && headers.Elements() != null)
            {
                var results = from parameter in headers.Elements()
                              where parameter.HasAttributes
                              select new
                              {
                                  Key = parameter.Attribute("name").Value,
                                  Value = parameter.Attribute("value").Value
                              };
                // TODO: Map headers against LINQ result and check for data types
            }
            var outputTranslationElement = configuration.XPathSelectElement("./Output/Translate");
            if (outputTranslationElement != null)
                OutputTranslation = Translators.Factory.Build(absolutePath, relativePath, outputTranslationElement);
            var cacheTranslationElement = configuration.XPathSelectElement("./Output/Cache");
            if (cacheTranslationElement != null)
            {
                CacheKey = cacheTranslationElement.Attribute("key").Value;
                CacheType = cacheTranslationElement.Attribute("type").Value;
            }
            var timeoutElement = configuration.XPathSelectElement("./Timeout");
            if (timeoutElement != null)
                CommandTimeout = int.Parse(timeoutElement.Value);
        }
    }
}
