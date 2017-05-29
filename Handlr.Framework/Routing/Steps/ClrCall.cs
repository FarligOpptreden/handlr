using Handlr.Framework.Routing.Attributes;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Loaders;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Web.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that executes a method within the assembly.
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
        /// Performs the execution of the CLR method.
        /// </summary>
        /// <param name="fieldCache">The field cache to use during the conditional check</param>
        /// <returns>An updated field cache containing data returned by all child steps</returns>
        /// <exception cref="ArgumentNullException">Thrown when the fieldCache parameter is null</exception>
        public override IFieldCache ExecuteStep(IFieldCache fieldCache)
        {
            if (fieldCache == null)
                throw new ArgumentNullException("fieldCache");

            object classInstance = null;

            try
            {
                // Parse the constructor arguments
                var constructorArgs =
                    (
                        from arg in LoaderArguments.ConstructorArguments
                        select arg.SetValue(ParseValue(arg.ValueKey))
                    );
                // Create an instance of the CLR type
                classInstance = Activator.CreateInstance(LoaderArguments.Type, constructorArgs.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                // A reflection exception occurred, so grab the inner (actual) exception
                throw new ParserException(string.Format("A new instance of type \"{0}\" could not be instantiated: {1}", LoaderArguments.Method.Name, ex.InnerException.Message));
            }
            catch (Exception ex)
            {
                // A general, unhandled exception occurred
                throw new ParserException(string.Format("A new instance of type \"{0}\" could not be instantiated: {1}", LoaderArguments.Method.Name, ex.Message));
            }

            if (classInstance == null)
                // The type could not be instantiated, so throw an exception to exit the step
                throw new ParserException(string.Format("A new instance of type \"{0}\" could not be instantiated", LoaderArguments.Type.Name));

            object result = null;

            try
            {
                // Parse the method arguments
                var methodArgs =
                (
                    from arg in LoaderArguments.MethodArguments
                    select arg.SetValue(ParseValue(arg.ValueKey))
                );
                // Invoke the method and get the output
                result = LoaderArguments.Method.Invoke(classInstance, methodArgs.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                // A reflection exception occurred, so grab the inner (actual) exception
                throw new ParserException(string.Format("The method \"{0}\" on type \"{1}\" could not be executed: {2}", LoaderArguments.Method.Name, LoaderArguments.Type.Name, ex.InnerException.Message));
            }
            catch (Exception ex)
            {
                // A general, unhandled exception occurred
                throw new ParserException(string.Format("The method \"{0}\" on type \"{1}\" could not be executed: {2}", LoaderArguments.Method.Name, LoaderArguments.Type.Name, ex.Message));
            }

            if (result != null && result.GetType() == typeof(void))
                // The return type of the method is "void", so there is no field cache to set
                return fieldCache;

            // Set the output in the field cache and return the updated cache
            fieldCache.Add(LoaderArguments.OutputKey, result);
            return fieldCache;
        }
    }
}
