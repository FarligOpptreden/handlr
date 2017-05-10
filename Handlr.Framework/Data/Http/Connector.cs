using Handlr.Framework.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Handlr.Framework.Data.Http
{
    public abstract class Connector : ISource<string, Connection>, IConnector, IDisposable
    {
        public string LoggingPath { get; set; }
        public string ConnectionString { get; set; }
        public Connection Connection { get; set; }
        public bool BubbleExceptions { get; set; }

        public Connector()
        {
        }

        public Connector(string url)
        {
            ConnectionString = url;
        }

        public virtual void Initialize() { }

        public bool TestConnection()
        {
            return Connection.Test();
        }

        public void SetContentType(string contentType)
        {
            Connection.ContentType = contentType;
        }

        public void SetAcceptType(string contentType)
        {
            Connection.AcceptType = contentType;
        }

        public string Call(object body, Dictionary<string, object> parameters = null, Dictionary<string, string> headers = null)
        {
            Command command = Prepare(null, parameters, body, headers);
            return command.Execute(BubbleExceptions);
        }

        public IEnumerable<T> Call<T>(object body, Dictionary<string, object> parameters = null, Dictionary<string, string> headers = null)
            where T : new()
        {
            string result = Call(body, parameters, headers);
            if (!string.IsNullOrEmpty(result))
            {
                T obj = result.To<T>();
                if (obj != null && obj is IEnumerable<T>)
                    return obj as IEnumerable<T>;
                if (obj != null && obj is T)
                    return new T[] { obj };
            }
            return null;
        }

        public void ExecuteWriter(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0)
        {
            Command command = Prepare(query, inputParameters);
            command.Execute(BubbleExceptions);
        }

        public string ExecuteReader(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, bool alwaysReturnData = false, int commandTimeout = 0)
        {
            Command command = Prepare(query, inputParameters);
            return command.Execute(BubbleExceptions);
        }

        public IEnumerable<T> ExecuteTypedReader<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null, int commandTimeout = 0)
             where T : class, new()
        {
            string result = ExecuteReader(query, inputParameters, null, false, commandTimeout) as string;
            if (!string.IsNullOrEmpty(result))
            {
                T obj = result.To<T>();
                if (obj != null && obj is IEnumerable<T>)
                    return obj as IEnumerable<T>;
                if (obj != null && obj is T)
                    return new T[] { obj };
            }
            return null;
        }

        public T ExecuteScalar<T>(string query, Dictionary<string, object> inputParameters, Dictionary<string, object> outputParameters = null)
        {
            Command command = Prepare(query, inputParameters);
            string result = command.Execute(BubbleExceptions);
            // parse result to T
            return default(T);
        }

        protected virtual Command Prepare(string query, Dictionary<string, object> parameters)
        {
            return Prepare(query, parameters, null);
        }

        protected virtual Command Prepare(string query, Dictionary<string, object> parameters, object body, Dictionary<string, string> headers = null)
        {
            if (Connection == null)
                throw new Exception("Database connector hasn't been initialized yet. Call .Initialize() first.");
            Command command = new Command()
            {
                CommandText = query,
                Connection = Connection
            };
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> kvp in parameters)
                {
                    command.Parameters.Add(new Parameter()
                    {
                        ParameterName = kvp.Key,
                        Value = kvp.Value ?? DBNull.Value
                    });
                }
                parameters = null;
            }
            if (headers != null)
                foreach (KeyValuePair<string, string> header in headers)
                {
                    command.Headers.Add(header.Key, header.Value);
                }
            command.RequestBody = body;
            command.Prepare();
            return command;
        }

        public void Dispose()
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
    }
}
