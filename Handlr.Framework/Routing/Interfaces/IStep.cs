namespace Handlr.Framework.Routing.Interfaces
{
    /// <summary>
    /// An interface for building steps executed in an IRoute implementation.
    /// </summary>
    public interface IStep
    {
        void Load<T>(T loaderArgs) where T : ILoaderArguments;

        T Execute<T>(T fieldCache) where T : IFieldCache;
    }
}
