using Handlr.Framework.Web.Interfaces;
using System.Collections.Generic;

namespace Handlr.Framework.Routing.Interfaces
{
    /// <summary>
    /// An interface for building a route.
    /// </summary>
    /// <typeparam name="FC">The type of field cache to use</typeparam>
    /// <typeparam name="LA">The type of loader arguments to use</typeparam>
    /// <typeparam name="I">The type of input data to use</typeparam>
    /// <typeparam name="O">The type of output data returned</typeparam>
    public interface IProcess<FC, LA, I, O>
        where FC : IFieldCache
        where LA : ILoaderArguments
        where I : IInput
        where O : IOutput
    {
        IList<IStep> Steps { get; }

        LA LoaderArguments { get; }

        ITranslation InputTranslation { get; }

        ITranslation OutputTranslation { get; }

        void Load(IController executionContext, LA loaderArgs);

        O Execute(I input);
    }
}
