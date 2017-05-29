using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Routing.Loaders;
using System;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Web.Interfaces;
using Handlr.Framework.Routing.Exceptions;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that executes a method within the assembly
    /// </summary>
    [Tag("ClrCall")]
    public class ClrCall : Base<ClrCallLoaderArguments>
    {
        /// <summary>
        /// Creates a new ClrCall instance.
        /// </summary>
        /// <param name="executionContext">The current execution context of the step</param>
        public ClrCall(IController executionContext) : base(executionContext) { }

        /// <summary>
        /// Performs the execution of the clr method.
        /// </summary>
        /// <param name="fieldCache">The field cache to use during the conditional check</param>
        /// <returns>An updated field cache containing data returned by all child steps</returns>
        /// <exception cref="ArgumentNullException">Thrown when the fieldCache parameter is null</exception>
        public override IFieldCache ExecuteStep(IFieldCache fieldCache)
        {
            if (fieldCache == null)
                throw new ArgumentNullException("fieldCache");

            var classInstance = Activator.CreateInstance(LoaderArguments.Type, LoaderArguments.ConstructorArguments);

            if (classInstance == null)
                throw new ParserException(string.Format("A new instance of type \"{0}\" could not be instantiated", LoaderArguments.Type.Name));

            LoaderArguments.Method.Invoke(classInstance, LoaderArguments.MethodArguments.ToArray());

            return fieldCache;
        }
    }
}
