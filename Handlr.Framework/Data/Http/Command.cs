using System;
using System.Linq;
using System.Web;
using Handlr.Framework.Web.Types;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Handlr.Framework.Data.Http
{
    public class Command : System.Data.Common.DbCommand
    {
        private ParameterCollection _Parameters = null;

        public override string CommandText { get; set; }

        public override int CommandTimeout { get; set; } = 30000;

        public override System.Data.CommandType CommandType { get; set; }

        protected Connection _Connection;

        protected override System.Data.Common.DbConnection DbConnection
        {
            get
            {
                return _Connection;
            }
            set
            {
                if (value.GetType() != typeof(Connection))
                    throw new Exception("Please use a Handlr.Data.Http.Connection type.");
                _Connection = (Connection)value;
            }
        }

        protected override System.Data.Common.DbParameterCollection DbParameterCollection { get { return _Parameters; } }

        protected override System.Data.Common.DbTransaction DbTransaction { get; set; }

        public override System.Data.UpdateRowSource UpdatedRowSource { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public object RequestBody { get; set; }

        public Dictionary<string, string> Headers
        {
            get
            {
                return _Connection != null ? _Connection.Headers : null;
            }
        }

        public Command()
            : base()
        {
            _Parameters = new ParameterCollection();
        }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        protected override System.Data.Common.DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
        {
            return base.CreateObjRef(requestedType);
        }

        protected override System.Data.Common.DbDataReader ExecuteDbDataReader(System.Data.CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public virtual string Execute(bool bubbleException = false)
        {
            string result = "";
            try
            {
                Connection.Open();
                ((Connection)Connection).Request.Timeout = CommandTimeout;
                result = ((Connection)Connection).Read();
                Connection.Close();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, System.Web.Configuration.WebConfigurationManager.AppSettings["HANDLR:ERRORLOG"]);
                if (bubbleException)
                    throw ex;
            }
            finally
            {
                if (Connection != null && Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }
            return result;
        }

        public override void Prepare()
        {
            if (!string.IsNullOrEmpty(CommandText))
                Connection.ConnectionString += "/" + CommandText;
            var modifiedParameters = new Func<List<Parameter>>(() =>
            {
                List<Parameter> parameters = new List<Parameter>();
                foreach (Parameter parameter in _Parameters)
                    parameters.Add(parameter);
                return parameters;
            })();
            foreach (Parameter parameter in _Parameters)
            {
                if (Connection.ConnectionString.Contains("{" + parameter.ParameterName + "}"))
                {
                    Connection.ConnectionString = Connection.ConnectionString.Replace("{" + parameter.ParameterName + "}", HttpUtility.UrlEncode(parameter.Value.ToString()));
                    modifiedParameters.Remove(parameter);
                }
            }
            if (new Method[] { Method.Get, Method.Delete }.Contains((Connection as Connection).Method))
            {
                if (modifiedParameters != null && modifiedParameters.Count > 0)
                {
                    string Url = Connection.ConnectionString;
                    if (!Url.Contains("?"))
                        Url += "?";
                    else
                        Url += "&";
                    foreach (Parameter parameter in modifiedParameters)
                    {
                        Url += HttpUtility.UrlEncode(parameter.ParameterName) + "=" + HttpUtility.UrlEncode(parameter.Value.ToString()) + "&";
                    }
                    Url = Url.Substring(0, Url.Length - 1);
                    Connection.ConnectionString = Url;
                }
            }
            if (new Method[] { Method.Post, Method.Put }.Contains((Connection as Connection).Method))
            {
                ((Connection)Connection).Headers.Add("Content-Length", "0");
                if (RequestBody != null)
                {
                    string body = "";
                    if (RequestBody.IsPrimitive() && string.IsNullOrEmpty(((Connection)Connection).ContentType))
                    {
                        body = HttpUtility.UrlEncode(RequestBody.ToString());
                        ((Connection)Connection).ContentType = AllTypes.StringFromContentTypes(ContentType.PlainText);
                    }
                    else if (RequestBody.GetType() == typeof(XmlDocument))
                    {
                        body = HttpUtility.UrlEncode(((XmlDocument)RequestBody).OuterXml);
                        ((Connection)Connection).ContentType = AllTypes.StringFromContentTypes(ContentType.Xml);
                    }
                    else if (RequestBody.GetType() == typeof(XDocument))
                    {
                        body = HttpUtility.UrlEncode(((XDocument)RequestBody).ToString());
                        ((Connection)Connection).ContentType = AllTypes.StringFromContentTypes(ContentType.Xml);
                    }
                    else if (!RequestBody.IsPrimitive())
                    {
                        body = RequestBody.ToJson();
                        ((Connection)Connection).ContentType = AllTypes.StringFromContentTypes(ContentType.Json);
                    }
                    else
                    {
                        body = RequestBody.ToString();
                    }
                    ((Connection)Connection).Headers["Content-Length"] = body.Length.ToString();
                    ((Connection)Connection).Write(body);
                    return;
                }
                if (Parameters.Count == 0)
                    return;
                string postBody = ((Connection)Connection).ContentType.Contains("json") ? "{" : "";
                foreach (Parameter parameter in modifiedParameters)
                {
                    if (((Connection)Connection).ContentType.Contains("json"))
                        postBody += "\"" + parameter.ParameterName + "\" : \"" + parameter.Value.ToString() + "\",";
                    else
                        postBody += parameter.ParameterName + "=" + parameter.Value.ToString() + "&";
                }
                postBody = postBody.Substring(0, postBody.Length - 1);
                postBody += ((Connection)Connection).ContentType.Contains("json") ? "}" : "";
                ((Connection)Connection).Headers["Content-Length"] = postBody.Length.ToString();
                ((Connection)Connection).Write(postBody);
            }
        }
    }
}
