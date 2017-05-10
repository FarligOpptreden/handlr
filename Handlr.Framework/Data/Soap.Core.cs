using System;
using System.Collections.Generic;

namespace Handlr.Framework.Data.Soap
{
    public sealed class Core : Http.Connector
    {
        private Core(string url)
            : base(url)
        {
            throw new Exception("Under construction");
        }

        public override void Initialize()
        {
            Connection = new Connection(ConnectionString);
        }

        protected override Http.Command Prepare(string query, Dictionary<string, object> parameters)
        {
            /*
            base.Prepare<C, P>(query, parameters);
            Request.ContentType = "text/xml; charset=\"utf-8\"";
            ((HttpWebRequest)Request).Accept = "text/xml";

            string soapEnvelope = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body>";
            soapEnvelope += "<" + query + " xmlns=\"http://tempuri.org/\">";
            foreach (KeyValuePair<string, object> kvp in parameters)
            {
                soapEnvelope += "<" + kvp.Key + ">" + kvp.Value.ToString() + "</" + kvp.Key + ">";
            }
            soapEnvelope += "</" + query + ">";
            soapEnvelope += "</soap:Body></soap:Envelope>";
            Request.ContentLength = soapEnvelope.Length;
            using (StreamWriter writer = new StreamWriter(Request.GetRequestStream()))
            {
                writer.Write(soapEnvelope);
                writer.Flush();
            }
             */
            return base.Prepare(query, parameters);
        }
    }
}
