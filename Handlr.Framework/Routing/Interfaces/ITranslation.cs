namespace Handlr.Framework.Routing.Interfaces
{
    /// <summary>
    /// An interface for building data translations.
    /// </summary>
    public interface ITranslation
    {
        void Load<T>(T loaderArgs)
            where T : ILoaderArguments;

        IOutput Translate(IInput input);
    }
}
