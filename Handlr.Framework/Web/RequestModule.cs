using System;
using System.Web;
using System.IO;

namespace Handlr.Framework.Web
{
    /// <summary>
    /// A request module that is registered in IIS to forward incoming requests to the RequestHandler HTTP handler.
    /// </summary>
    public class RequestModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.PostMapRequestHandler += new EventHandler(PostMapRequestHandler);
            application.PreSendRequestHeaders += new EventHandler(PreSendRequestHeaders);
        }

        private void PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            context.Response.Headers.Remove("X-AspNet-Version");
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");
        }

        private void PostMapRequestHandler(object sender, EventArgs e)
        {
            // Get the current HTTP context.
            HttpContext context = ((HttpApplication)sender).Context;
            // If the file doesn't exist on disk, instantiate the RequestHandler and set the executing context to its implementation.
            if (!File.Exists(context.Server.MapPath(context.Request.AppRelativeCurrentExecutionFilePath)))
            {
                IHttpHandler handler = new RequestHandler();
                context.Handler = handler;
            }
        }

        public void Dispose()
        {
        }
    }
}