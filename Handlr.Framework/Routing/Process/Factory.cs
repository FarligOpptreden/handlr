using Handlr.Framework.Web;
using Handlr.Framework.Web.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Handlr.Framework.Routing.Process
{
    /// <summary>
    /// A factory for creating REST processes.
    /// </summary>
    public static class Factory
    {
        const string RoutesFolder = "_routes";

        /// <summary>
        /// Builds a REST process using the supplied parameters.
        /// </summary>
        /// <param name="factoryArguments">The arguments used to construct the REST process</param>
        /// <returns>An initialized REST process</returns>
        /// <exception cref="ArgumentNullException">Thrown when the factoryArguments parameter is null</exception>
        /// <exception cref="WebException">
        /// Thrown when either:
        ///   no configuration file is found for the route;
        ///   the configured route doesn't match the resource path;
        ///   the configured method doesn't match the resource access method or
        ///   the configured producing type does not match the requested content type
        /// </exception>
        public static RestProcess Build(RestProcessFactoryArguments factoryArguments)
        {
            if (factoryArguments == null)
                throw new ArgumentNullException("factoryArguments");

            string routeString = string.Format("[{0}]/{1}({2})", factoryArguments.RequestMethod.ToString().ToUpper(), factoryArguments.RequestUrlSegments.Flatten("/"), factoryArguments.RequestProduces.ToString().ToLower());
            // Build up the path to the definition file according to this template: ACCESSMETHOD-ACCEPTHEADER-path-segments.xml
            string definitionFile = string.Format("{0}-{1}-{2}.xml",
                factoryArguments.MappedRequestMethod,
                factoryArguments.MappedRequestProduces,
                factoryArguments.MappedRequestUrlSegments);
            FileInfo fi = new FileInfo(Path.Combine(
                factoryArguments.ModulePath,
                factoryArguments.ModuleName,
                RoutesFolder,
                definitionFile));

            // A configuration with the file name doesn't exist, so find one based on path variables
            if (!fi.Exists)
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(
                    factoryArguments.ModulePath,
                    factoryArguments.ModuleName,
                    RoutesFolder));

                if (!di.Exists)
                    throw new WebException(Status.NotFound, string.Format("No configuration exists for the module {0}.", factoryArguments.ModuleName));

                // Enumarate through all files in the module's routes folder to find files with path variables matching the incoming route
                foreach (var file in di.EnumerateFiles())
                {
                    var matches = Regex.Matches(file.Name, "\\{[a-zA-Z0-9_\\-]+\\}", RegexOptions.IgnoreCase);
                    if (matches == null || matches.Count == 0)
                        continue;
                    string fileName = file.Name;
                    List<string> pathVariables = new List<string>();
                    foreach (Match match in matches)
                    {
                        fileName = fileName.Replace(match.Value, "(.+)");
                        pathVariables.Add(match.Value.Substring(1, match.Length - 2));
                    }
                    var fileMatch = Regex.Match(definitionFile, fileName, RegexOptions.IgnoreCase);
                    if (fileMatch == null || !fileMatch.Success)
                        continue;
                    for (int i = 1; i < fileMatch.Groups.Count; i++)
                    {
                        Group group = fileMatch.Groups[i];
                        string key = pathVariables[i - 1];
                        string value = group.Value;
                        factoryArguments.RequestPathVariables.Add(key, value);
                    }
                    fi = file;
                    break;
                }
            }

            // A configuration doesn't exist for the route, so exit with a 404 web exception
            if (!fi.Exists)
                throw new WebException(Status.NotFound, string.Format("No configuration exists for the route {0}.", routeString));

            // Load the configuration attributes
            XDocument definition = XDocument.Load(fi.FullName);
            string url = definition.XPathEvaluate("string(/Definition/@url)") as string;
            string method = definition.XPathEvaluate("string(/Definition/@method)") as string;
            string consumes = definition.XPathEvaluate("string(/Definition/@consumes)") as string;
            string produces = definition.XPathEvaluate("string(/Definition/@produces)") as string;

            foreach (var pathVar in factoryArguments.RequestPathVariables)
                url = url.Replace("{" + pathVar.Key + "}", pathVar.Value);

            // The configuration's @url attribute does not match the route, so exit with a 404 web exception
            if (string.IsNullOrEmpty(url) || url != "/" + factoryArguments.RequestUrlSegments.Flatten("/"))
                throw new WebException(Status.NotFound, string.Format("The route {0} does not match the configured resource path.", routeString));

            // The configuration's @method attribute does not match the route's access method, so exit with a 405 web exception
            if (!string.IsNullOrEmpty(method) && method.ToUpper() != factoryArguments.MappedRequestMethod)
                throw new WebException(Status.MethodNotAllowed, string.Format("The access method for the route {0} does not match the configured access method.", routeString));

            // The configuration's @produces attribute does not match the route's accept header, so exit with a 415 web exception
            if (!string.IsNullOrEmpty(produces) && produces.ToLower() != AllTypes.StringFromContentTypes(factoryArguments.RequestProduces))
                throw new WebException(Status.UnsupportedMediaType, string.Format("The accept header of the route {0} does not match the configured response type.", routeString));

            // Initialize the process before returning
            var restProcess = new RestProcess();
            var loaderArgs = new Loaders.RestProcessLoaderArguments(Path.Combine(factoryArguments.ModulePath, factoryArguments.ModuleName), factoryArguments.Module, definition.XPathSelectElement("/Definition"));
            loaderArgs.FactoryArgs = factoryArguments;
            restProcess.Load(factoryArguments.Handler, loaderArgs);

            return restProcess;
        }
    }
}
