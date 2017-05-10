using Handlr.Framework.Data.Interfaces;
using Handlr.Framework.Web.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Handlr.Framework.Data
{
    /// <summary>
    /// Provides a base for creating database connector classes. This class cannot be instantiated.
    /// </summary>
    public abstract class Connector : ISource<List<DataTable>, DbConnection>, IConnector, IDisposable
    {
        public string ConnectionString { get; set; }
        public DbConnection Connection { get; set; }
        public string LoggingPath { get; set; }
        public List<DataTable> LastResults { get; set; }
        public Dictionary<string, object> LastOutputParameters { get; set; }
        public bool BubbleExceptions { get; set; }

        /// <summary>
        /// Constructs a new database connector.
        /// </summary>
        protected Connector()
        {
            LoggingPath = System.Web.Configuration.WebConfigurationManager.AppSettings["HANDLR:ERRORLOG"];
        }

        /// <summary>
        /// Constructs a new database connector and sets the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to the datasource</param>
        protected Connector(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Releases all resources used by the database connector.
        /// </summary>
        public virtual void Dispose()
        {
            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
                Connection.Dispose();
            }
            Connection = null;
            ConnectionString = null;
        }

        /// <summary>
        /// Tests the connection to the database connector.
        /// </summary>
        /// <returns>A value indicating whether the connection could successfully be created</returns>
        public virtual bool TestConnection()
        {
            try
            {
                Connection.Open();
            }
            catch (Exception ex)
            {
                Logging.Log(new Exception(string.Format("Could test connection to \"{0}\": {1}", Connection.ConnectionString, ex.Message)), LoggingPath);
                return false;
            }
            if (Connection != null && Connection.State == ConnectionState.Open)
                Connection.Close();
            return true;
        }

        private void SetOutputParameters<C, P>(C command)
            where C : DbCommand
            where P : DbParameter
        {
            if (command.Parameters != null)
            {
                LastOutputParameters = new Dictionary<string, object>();
                foreach (P parameter in command.Parameters)
                {
                    if (parameter.Direction == ParameterDirection.Output)
                        LastOutputParameters.Add(parameter.ParameterName, parameter.Value);
                }
            }
        }

        /// <summary>
        /// Executes a command that modifies data in a database.
        /// </summary>
        /// <typeparam name="C">The type of command to execute</typeparam>
        /// <param name="command">The command that should be executed</param>
        /// <returns>A value indicating whether the query executed successfully or not</returns>
        protected bool _ExecuteWriter<C, P>(C command)
            where C : DbCommand
            where P : DbParameter
        {
            bool success = true;
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                command.ExecuteNonQuery();
                Connection.Close();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LoggingPath);
                success = false;
                if (BubbleExceptions)
                    throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            SetOutputParameters<C, P>(command);
            command.Dispose();
            command = null;
            return success;
        }

        /// <summary>
        /// Executes a command that reads data from a database, returning 1 or more result set.
        /// </summary>
        /// <typeparam name="A">The type of data adapter to use</typeparam>
        /// <param name="dataAdapter">The data adapter used to fill the data set</param>
        /// <returns>A list of strongly typed data tables, one for each result set in the query(ies) of the command</returns>
        protected List<DataTable> _ExecuteReader<C, P, R>(C command, bool alwaysReturnData = false)
            where C : DbCommand
            where P : DbParameter
            where R : DbDataReader
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                LastResults = new List<DataTable>();
                using (R reader = (R)command.ExecuteReader())
                {
                    bool iterate = alwaysReturnData || reader.HasRows;
                    while (iterate)
                    {
                        var table = new DataTable("Table " + LastResults.Count);
                        for (int i = 0; i < reader.FieldCount; i++)
                            table.Columns.Add(new DataColumn(reader.GetName(i), reader.GetFieldType(i)));
                        LastResults.Add(table);
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                object[] values = new object[reader.FieldCount];
                                reader.GetValues(values);
                                table.Rows.Add(values);
                            }
                        iterate = reader.NextResult();
                    }
                }
                Connection.Close();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LoggingPath);
                if (BubbleExceptions)
                    throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            SetOutputParameters<C, P>(command);
            command.Dispose();
            command = null;
            return LastResults;
        }

        public IEnumerable<T> ConstructFromTable<T>(DataTable table, string colPrefix = null)
            where T : class
        {
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    yield return ConstructFromRow<T>(row, "");
                }
            }
            yield break;
        }

        public T ConstructFromRow<T>(DataRow row, string colPrefix = null)
            where T : class
        {
            return ConstructObject(typeof(T), row, colPrefix) as T;
        }

        private object ConstructObject(Type type, DataRow row, string colPrefix)
        {
            PropertyInfo[] fields = type.GetProperties();
            var obj = Activator.CreateInstance(type);
            foreach (var field in fields)
            {
                if (row.Table.Columns[colPrefix + field.Name] != null)
                {
                    var value = row[colPrefix + field.Name];
                    if (value == DBNull.Value)
                        value = field.PropertyType.DefaultValue();
                    field.SetValue(obj, value);
                    continue;
                }
                var mapsTo = field.GetCustomAttribute<MapsTo>();
                if (mapsTo != null && row.Table.Columns[colPrefix + mapsTo.Value] != null)
                {
                    var value = row[colPrefix + mapsTo.Value];
                    if (value == DBNull.Value)
                        value = field.PropertyType.DefaultValue();
                    field.SetValue(obj, value);
                    continue;
                }
                if (mapsTo != null && mapsTo.Value.EndsWith("*") && !field.IsPrimitive())
                {
                    field.SetValue(obj, ConstructObject(field.PropertyType, row, colPrefix + mapsTo.Value.Replace("*", "")));
                }
            }
            return obj;
        }

        protected IEnumerable<T> _ExecuteTypedReader<T, C, P, R>(C command)
            where C : DbCommand
            where P : DbParameter
            where R : DbDataReader
            where T : class, new()
        {
            List<DataTable> results = _ExecuteReader<C, P, R>(command);
            LastResults = results;
            return ConstructFromTable<T>(results != null && results.Count > 0 ? results[0] : null);
        }

        /// <summary>
        /// Executes a command that reads a single value from a database.
        /// </summary>
        /// <typeparam name="T">The type of data to be returned</typeparam>
        /// <typeparam name="C">The type of command to execute</typeparam>
        /// <param name="command">The command that should be executed</param>
        /// <returns>The single value read from the database</returns>
        protected T _ExecuteScalar<T, C>(C command)
            where C : DbCommand
        {
            T result = default(T);
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                result = (T)command.ExecuteScalar();
                Connection.Close();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, LoggingPath);
                if (BubbleExceptions)
                    throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Prepares a command for execution.
        /// </summary>
        /// <typeparam name="C">The type of command to prepare</typeparam>
        /// <typeparam name="P">The type of parameters to add to the command</typeparam>
        /// <param name="query">The query to be prepared for the command</param>
        /// <param name="parameters">Dictionary of parameters to be added to the command</param>
        /// <returns>A prepared command to execute against a database</returns>
        protected virtual C Prepare<C, P>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0)
            where C : DbCommand, new()
            where P : DbParameter, new()
        {
            if (Connection == null)
                throw new Exception("Database connector hasn't been initialized yet. Call .Initialize() first.");
            C command = new C()
            {
                CommandText = query,
                Connection = Connection
            };
            command.CommandTimeout = commandTimeout;
            if (inputParameters != null)
            {
                foreach (KeyValuePair<string, object> kvp in inputParameters)
                {
                    command.Parameters.Add(new P()
                    {
                        ParameterName = kvp.Key,
                        Value = kvp.Value ?? DBNull.Value,
                        Direction = ParameterDirection.Input
                    });
                }
                inputParameters = null;
                if (outputParameters != null)
                {
                    var outputSize = new Func<object, int>(type =>
                    {
                        switch (type.ToString().ToLower())
                        {
                            case "system.int32": return 4000;
                            case "system.int64": return 4000;
                            case "system.string": return 4000;
                            case "system.boolean": return 4000;
                            case "system.datetime": return 4000;
                            case "system.double": return 4000;
                            default: return 4000;
                        }
                    });
                    foreach (KeyValuePair<string, object> kvp in outputParameters)
                    {
                        command.Parameters.Add(new P()
                        {
                            ParameterName = kvp.Key,
                            Value = kvp.Value ?? DBNull.Value,
                            Direction = ParameterDirection.Output,
                            Size = kvp.Value != null ? outputSize(kvp.Value.GetType()) : 4000
                        });
                    }
                    outputParameters = null;
                }
            }
            return command;
        }

        public abstract void Initialize();

        public abstract void ExecuteWriter(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0);

        public abstract List<DataTable> ExecuteReader(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, bool alwaysReturnData = false, int commandTimeout = 0);

        public abstract IEnumerable<T> ExecuteTypedReader<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0) where T : class, new();

        public abstract T ExecuteScalar<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null);
    }
}
