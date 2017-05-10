using System.Collections.Generic;

namespace Handlr.Framework.Data.Interfaces
{
    interface ISource<S, C>
    {
        void ExecuteWriter(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0);
        S ExecuteReader(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, bool alwaysReturnData = false, int commandTimeout = 0);
        IEnumerable<T> ExecuteTypedReader<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0) where T : class, new();
        T ExecuteScalar<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null);
        /// <summary>
        /// Represents a connection to the datasource against which commands can be executed.
        /// </summary>
        C Connection { get; set; }
    }
}
