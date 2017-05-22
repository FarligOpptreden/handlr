namespace Handlr.Framework.Routing.Interfaces
{
    /// <summary>
    /// An interface for building configurations to be passed to the routing engine.
    /// </summary>
    public interface IConfig
    {
        string PassPhrase { get; set; }
    }
}
