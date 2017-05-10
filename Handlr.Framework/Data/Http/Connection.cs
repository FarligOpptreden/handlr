using System;
using System.Text;
using System.Net;
using System.IO;
using Handlr.Framework.Web.Types;
using System.Collections.Generic;

namespace Handlr.Framework.Data.Http
{
    public class Connection : System.Data.Common.DbConnection
    {
        private System.Data.ConnectionState _State { get; set; }

        public override string ConnectionString { get; set; }

        internal string ContentType { get; set; }

        internal string AcceptType { get; set; }

        internal Method Method { get; private set; }

        internal WebRequest Request { get; private set; }

        internal WebResponse Response { get; private set; }

        internal Dictionary<string, string> Headers { get; private set; }

        public override string Database
        {
            get { return null; }
        }

        public override string DataSource
        {
            get { return null; }
        }

        public override string ServerVersion
        {
            get { return null; }
        }

        public override System.Data.ConnectionState State
        {
            get { return _State; }
        }

        public Connection(string url, Method method, string contentType = "application/json; charset=UTF-8")
        {
            ConnectionString = url;
            Method = method;
            ContentType = contentType;
            Headers = new Dictionary<string, string>();
        }

        protected override System.Data.Common.DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            if (Request != null)
                Request.Abort();
            Request = null;
        }

        protected override System.Data.Common.DbCommand CreateDbCommand()
        {
            throw new NotImplementedException();
        }

        public override void Open()
        {
            try
            {
                if (_State == System.Data.ConnectionState.Open)
                    return;
                _State = System.Data.ConnectionState.Open;
                Request = WebRequest.Create(ConnectionString);
                Request.Method = Method.ToString().ToUpper();
                Request.UseDefaultCredentials = true;
                Request.ContentType = ContentType;
                if (!string.IsNullOrEmpty(AcceptType))
                    ((HttpWebRequest)Request).Accept = AcceptType;
                foreach (KeyValuePair<string, string> header in Headers)
                {
                    if (Request.Headers[header.Key] != null)
                        continue;
                    if (header.Key.ToLower() == "content-length")
                    {
                        Request.ContentLength = long.Parse(header.Value);
                        continue;
                    }
                    Request.Headers.Add(header.Key, header.Value);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, System.Web.Configuration.WebConfigurationManager.AppSettings["HANDLR:ERRORLOG"]);
                throw new Exception("Could not open http connection.");
            }
        }

        public bool Test()
        {
            try
            {
                Request = WebRequest.Create(ConnectionString);
                Request.UseDefaultCredentials = true;
                Request.ContentType = ContentType;
                Request.Method = Method.Head.ToString().ToLower();
                Request.GetResponse();
            }
            catch
            {
                return false;
            }
            Request.Abort();
            Request = null;
            return true;
        }

        public void Write(string content)
        {
            if (Request == null)
                Open();
            byte[] bytes = Encoding.ASCII.GetBytes(content);
            Request.ContentLength = bytes.Length;
            using (Stream writer = Request.GetRequestStream())
            {
                writer.Write(bytes, 0, bytes.Length);
                writer.Flush();
                writer.Close();
            }
        }

        public string Read()
        {
            try
            {
                Response = Request.GetResponse();
            }
            catch (Exception ex)
            {
                Logging.Log(ex, System.Web.Configuration.WebConfigurationManager.AppSettings["HANDLR:ERRORLOG"]);
                throw new Exception("Could not execute http call: " + ex.Message);
            }
            string result = null;
            using (StreamReader reader = new StreamReader(Response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            _State = System.Data.ConnectionState.Closed;
            return result;
        }
    }
}
