﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Handlr.Framework.Data
{
    /// <summary>
    /// Provides methods to read and write data from ADO-based datasources.
    /// </summary>
    public sealed class Ado : Connector
    {
        /// <summary>
        /// Constructs a new database connector.
        /// </summary>
        /// <param name="connectionString">The connection string to the ADO-based datasource</param>
        private Ado(string connectionString) :
            base(connectionString)
        {
        }

        /// <summary>
        /// Initializes the database connector by creating a connection to the datasource.
        /// </summary>
        public override void Initialize()
        {
            Connection = new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Executes a query that modifies data in an ADO-based datasource.
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <param name="parameters">The parameters to be added to the query</param>
        public override void ExecuteWriter(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0)
        {
            _ExecuteWriter<SqlCommand, SqlParameter>(Prepare<SqlCommand, SqlParameter>(query, inputParameters, outputParameters, commandTimeout));
        }

        /// <summary>
        /// Executes a query that reads one or more result sets from an ADO-based datasource.
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <param name="parameters">The parameters to be added to the query</param>
        /// <returns></returns>
        public override List<DataTable> ExecuteReader(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, bool alwaysReturnData = false, int commandTimeout = 0)
        {
            return _ExecuteReader<SqlCommand, SqlParameter, SqlDataReader>(Prepare<SqlCommand, SqlParameter>(query, inputParameters, outputParameters, commandTimeout), alwaysReturnData);
        }

        /// <summary>
        /// Executes a query that reads one a single result set from an ADO-based datasource, returning a typed list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <param name="query">The query to execute</param>
        /// <param name="parameters">The parameters to be added to the query</param>
        /// <returns></returns>
        public override IEnumerable<T> ExecuteTypedReader<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0)
        {
            return _ExecuteTypedReader<T, SqlCommand, SqlParameter, SqlDataReader>(Prepare<SqlCommand, SqlParameter>(query, inputParameters, outputParameters, commandTimeout));
        }

        /// <summary>
        /// Executes a query that reads a single value from an ADO-based datasource.
        /// </summary>
        /// <typeparam name="T">The type of value to read from the datasource</typeparam>
        /// <param name="query">The qyery to execute</param>
        /// <param name="parameters">The parameters to be added to the query</param>
        /// <returns></returns>
        public override T ExecuteScalar<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null)
        {
            return _ExecuteScalar<T, SqlCommand>(Prepare<SqlCommand, SqlParameter>(query, inputParameters, outputParameters));
        }
    }
}
