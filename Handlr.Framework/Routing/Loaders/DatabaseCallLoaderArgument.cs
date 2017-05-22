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
    /// Represents loader arguments for initializing a database call.
    /// </summary>
    public class DatabaseCallLoaderArguments : Base
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
        public List<DatabaseParameter> Parameters { get; private set; } = new List<DatabaseParameter>();

        /// <summary>
        /// Gets the translation to apply after retrieving data from the data source.
        /// </summary>
        public ITranslation OutputTranslation { get; private set; }

        /// <summary>
        /// Creates a new AdoRouteLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        public DatabaseCallLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            ConnectionString = configuration.XPathEvaluate("string(./ConnectionString/text())") as string;
            Query = configuration.XPathEvaluate("string(./Query/text())") as string;
            var parameters = configuration.XPathSelectElement("./Parameters");
            if (parameters != null && parameters.Elements() != null)
            {
                Parameters =
                    (
                        from parameter in parameters.Elements()
                        select new DatabaseParameter()
                        {
                            Name = parameter.Attribute("name").Value,
                            Value = parameter.Attribute("value").Value,
                            Type = (DatabaseParameter.DataType)Enum.Parse(typeof(DatabaseParameter.DataType), parameter.Attribute("type").Value, true),
                            Nullable = bool.Parse(parameter.Attribute("nullable").Value)
                        }
                    ).ToList();
            }
            var outputTranslationElement = configuration.XPathSelectElement("./OutputTranslation");
            if (outputTranslationElement != null)
                OutputTranslation = Translators.Factory.Build(absolutePath, relativePath, outputTranslationElement);
        }
    }
}
