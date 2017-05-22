using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents loader arguments for initializing an ADO route.
    /// </summary>
    public class AdoRouteLoaderArguments : Base
    {
        /// <summary>
        /// Gets the connection string for the data source.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the query to execute against the data source.
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// Gets the parameters to apply to the query.
        /// </summary>
        public List<AdoParameter> Parameters { get; private set; } = new List<AdoParameter>();

        /// <summary>
        /// Gets the translation to apply after retrieving data from the data source.
        /// </summary>
        public ITranslation PostTranslation { get; private set; }

        /// <summary>
        /// Creates a new AdoRouteLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public AdoRouteLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            ConnectionString = configuration.XPathEvaluate("string(./ConnectionString/text())") as string;
            Query = configuration.XPathEvaluate("string(./Query/text())") as string;
            var parameters = configuration.XPathSelectElement("./Parameters");
            if (parameters != null && parameters.Elements() != null)
            {
                Parameters =
                    (
                        from parameter in parameters.Elements()
                        select new AdoParameter()
                        {
                            Name = parameter.Attribute("name").Value,
                            Value = parameter.Attribute("value").Value,
                            Type = (AdoParameter.DataType)Enum.Parse(typeof(AdoParameter.DataType), parameter.Attribute("type").Value, true),
                            Nullable = bool.Parse(parameter.Attribute("nullable").Value)
                        }
                    ).ToList();
            }
            var postTranslationElement = configuration.XPathSelectElement("./PostTranslation");
            if (postTranslationElement != null)
                PostTranslation = Translators.Factory.Build(absolutePath, relativePath, postTranslationElement);
        }
    }
}
