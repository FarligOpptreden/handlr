namespace Handlr.Framework.Data.Interfaces
{
    public interface IConnector
    {
        void Initialize();
        bool TestConnection();
        string LoggingPath { get; set; }
        /// /// <summary>
        /// Represents a connection string to the datasource.
        /// </summary>
        string ConnectionString { get; set; }
        bool BubbleExceptions { get; set; }
    }
}
