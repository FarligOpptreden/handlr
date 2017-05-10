using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace Handlr.Framework.Data
{
    /// <summary>
    /// Provides methods to read and write data from ODBC-based datasources.
    /// </summary>
    public sealed class Odbc : Connector
    {
        /// <summary>
        /// Constructs a new database connector.
        /// </summary>
        /// <param name="connectionString">The connection string to the ODBC-based datasource</param>
        private Odbc(string connectionString) :
            base(connectionString)
        {
        }

        /// <summary>
        /// Initializes the database connector by creating a connection to the datasource.
        /// </summary>
        public override void Initialize()
        {
            Connection = new OdbcConnection(ConnectionString);
        }

        /// <summary>
        /// Executes a query that modifies data in an ODBC-based datasource.
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <param name="parameters">The parameters to be added to the query</param>
        public override void ExecuteWriter(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0)
        {
            _ExecuteWriter<OdbcCommand, OdbcParameter>(Prepare<OdbcCommand, OdbcParameter>(query, inputParameters, outputParameters, commandTimeout));
        }

        /// <summary>
        /// Executes a query that reads one or more result sets from an ODBC-based datasource.
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <param name="parameters">The parameters to be added to the query</param>
        /// <returns></returns>
        public override List<DataTable> ExecuteReader(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, bool alwaysReturnData = false, int commandTimeout = 0)
        {
            return _ExecuteReader<OdbcCommand, OdbcParameter, OdbcDataReader>(Prepare<OdbcCommand, OdbcParameter>(query, inputParameters, outputParameters, commandTimeout), alwaysReturnData);
        }

        /// <summary>
        /// Executes a query that reads one a single result set from an ODBC-based datasource, returning a typed list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <param name="query">The query to execute</param>
        /// <param name="parameters">The parameters to be added to the query</param>
        /// <returns></returns>
        public override IEnumerable<T> ExecuteTypedReader<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0)
        {
            return _ExecuteTypedReader<T, OdbcCommand, OdbcParameter, OdbcDataReader>(Prepare<OdbcCommand, OdbcParameter>(query, inputParameters, outputParameters, commandTimeout));
        }

        /// <summary>
        /// Executes a query that reads a single value from an ODBC-based datasource.
        /// </summary>
        /// <typeparam name="T">The type of value to read from the datasource</typeparam>
        /// <param name="query">The qyery to execute</param>
        /// <param name="parameters">The parameters to be added to the query</param>
        /// <returns></returns>
        public override T ExecuteScalar<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null)
        {
            return _ExecuteScalar<T, OdbcCommand>(Prepare<OdbcCommand, OdbcParameter>(query, inputParameters, outputParameters));
        }
    }
}
