using Handlr.Framework.Web.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Handlr.Framework.Routing.Interfaces;
using System.Reflection;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Loaders
{
    /// <summary>
    /// Represents the loader arguments for initializing a CLR call.
    /// </summary>
    public class ClrCallLoaderArguments : Base
    {
        public Type Type { get; private set; }

        public List<ClrParameter> ConstructorArguments { get; private set; } = new List<ClrParameter>();

        public MethodInfo Method { get; private set; }

        public List<ClrParameter> MethodArguments { get; private set; } = new List<ClrParameter>();

        public string OutputKey { get; private set; }

        /// <summary>
        /// Creates a new ClrCallLoaderArguments instance.
        /// </summary>
        /// <param name="absolutePath">The absolute path of the module</param>
        /// <param name="relativePath">The relative path of the module</param>
        /// <param name="configuration">The configuration markup for the loader arguments</param>
        /// <exception cref="ParserException">Thrown when the configuration could not be parsed</exception>
        public ClrCallLoaderArguments(string absolutePath, string relativePath, XElement configuration) : base(absolutePath, relativePath, configuration)
        {
            var typeNode = configuration.XPathSelectElement("./Type");

            if (typeNode == null)
                throw new ParserException("The \"Type\" element for the CLR call configuration was not specified");

            Type =
                (
                    from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    select assembly.GetType(typeNode.Value)
                ).FirstOrDefault();
            var constructorArgsNode = configuration.XPathSelectElement("./ConstructorArguments");
            if (constructorArgsNode != null)
                foreach (var arg in constructorArgsNode.XPathSelectElements("./Argument"))
                    ConstructorArguments.Add(new ClrParameter(arg.Attribute("type").Value, arg.Value));

            var methodNode = configuration.XPathSelectElement("./Method");

            if (methodNode == null)
                throw new ParserException("The \"Method\" element for the CLR call configuration was not specified");

            var methodArgsNode = configuration.XPathSelectElement("./MethodArguments");
            if (methodArgsNode != null)
                foreach (var arg in methodArgsNode.XPathSelectElements("./Argument"))
                    MethodArguments.Add(new ClrParameter(arg.Attribute("type").Value, arg.Value));
            var methodTypes = from type in MethodArguments
                              select type.Type;
            Method = Type.GetMethod(methodNode.Value, BindingFlags.Public | BindingFlags.Instance, null, methodTypes.ToArray(), null);

            var outputNode = configuration.XPathSelectElement("./Output");

            if (outputNode == null)
                throw new ParserException("The \"Output\" element for the CLR call configuration was not specified");

            OutputKey = outputNode.Attribute("key").Value;

        }
    }
}
