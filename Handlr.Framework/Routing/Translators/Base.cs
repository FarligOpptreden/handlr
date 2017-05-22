using System;
using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Represents a base class for building translators with default implementaions for the ITranslation interface.
    /// </summary>
    /// <typeparam name="L">The type of loader arguments to use</typeparam>
    /// <typeparam name="I">The type of input object to consume</typeparam>
    /// <typeparam name="O">The type of output object to produce</typeparam>
    public abstract class Base<L> : ITranslation
        where L : ILoaderArguments
    {
        /// <summary>
        /// Gets the loader arguments associated with the translation
        /// </summary>
        public L LoaderArguments { get; private set; }

        public void Load(L loaderArgs)
        {
            LoaderArguments = loaderArgs;
        }

        /// <summary>
        /// Loads the translation using the specified loader arguments.
        /// </summary>
        /// <typeparam name="T">The type of loader arguments to use</typeparam>
        /// <param name="loaderArgs">The loader arguments to load the translation with</param>
        public void Load<T>(T loaderArgs) where T : ILoaderArguments
        {
            if (loaderArgs == null)
                throw new ArgumentNullException("loaderArgs");

            Load(loaderArgs);
        }

        /// <summary>
        /// Performs the translation using the specified input.
        /// </summary>
        /// <param name="input">The input to use for the translation</param>
        /// <returns>The output produced by the translation</returns>
        public abstract IFieldCache Translate(IFieldCache input);

        /// <summary>
        /// Performs the translation using the specified input.
        /// </summary>
        /// <param name="input">The input to use for the translation</param>
        /// <typeparam name="TI">The type of input to consume</typeparam>
        /// <typeparam name="TO">The type of output to produce</typeparam>
        /// <returns>The output produced by the translation</returns>
        public TO Translate<TI, TO>(TI input)
            where TI : IFieldCache, IInput
            where TO : IFieldCache, IOutput
        {
            if (input == null)
                throw new ArgumentNullException("input");

            return (TO)Translate(input);
        }
    }
}
