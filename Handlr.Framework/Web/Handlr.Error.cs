using System;
using System.IO;
using System.Web;
using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Web
{
    internal sealed class Error : Handler
    {
        private Status _Status;
        private string _Message;
        private Exception _InnerException;

        private Error(HttpContext context, Config configuration, Status status, string message, Exception innerException)
            : base(context, configuration)
        {
            _Status = status;
            _Message = message;
            _InnerException = innerException;
        }

        private Error(HttpContext context, Config configuration, Status status, string message)
            : this(context, configuration, status, message, null)
        { }

        public void Handle()
        {
            string html = "";
            if (File.Exists(Server.MapPath("~/errors/" + (int)_Status + ".cshtml")))
            {
                ViewModel.SetModel(new WebException(_Status, _Message, _InnerException));
                html = LoadView("~/errors/" + (int)_Status);
            }
            else
            {
                html += "<!DOCTYPE html><html>";
                html += "<head><title>Handlr : Error " + (int)_Status + "</title></head>";
                html += "<body>";
                html += "<h1>Error " + (int)_Status + "</h1><hr>";
                html += "<pre>" + _Message + "</pre>";
                if (_InnerException != null)
                {
                    html += "<hr>";
                    html += "<h2>" + _InnerException.Message + "</h2>";
                    html += "<pre>" + _InnerException.StackTrace + "</pre>";
                }
                html += "</body>";
                html += "</html>";
            }
            WriteResponse(
                content: html,
                minify: false,
                statusCode: (int)_Status
                );
        }

        public static Error Factory(HttpContext context, dynamic configuration, string loggingPath, Status status, string message)
        {
            return Factory(context, configuration, loggingPath, status, message, null);
        }

        public static Error Factory(HttpContext context, dynamic configuration, string loggingPath, Status status, string message, Exception innerException)
        {
            string log = "[" + (int)status + "] " + message;
            if (context != null && context.Request != null)
                log += "\r\n                      > Url: " + context.Request.Url.AbsoluteUri +
                       "\r\n                      > Referrer Url: " + context.Request.UrlReferrer +
                       "\r\n                      > Origin IP: " + context.Request.UserHostAddress +
                       "\r\n                      > User Agent: " + context.Request.UserAgent;
            if (innerException != null && innerException.StackTrace != null)
            {
                log += "\r\n                      > Stack Trace:";
                foreach (string trace in innerException.StackTrace.Split('\n'))
                {
                    log += "\r\n                     " + trace;
                }
            }
            log += "\r\n";
            Logging.Log(new Exception(log), loggingPath);
            return new Error(context, configuration, status, message, innerException);
        }
    }
}
