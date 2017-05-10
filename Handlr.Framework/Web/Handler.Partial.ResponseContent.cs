using System;
using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Web
{
    public abstract partial class Handler
    {
        public string LoadView(string relativePath)
        {
            if (!Initialized)
                throw new Exception("The Handler hasn't been initialized yet. Call Initialize(HttpContext) before using the Handler.");
            if (relativePath.IndexOf(".cshtml") < 0 && relativePath.IndexOf(".cshtml") <= 0)
                relativePath += ".cshtml";
            ViewRenderer renderer = new ViewRenderer();
            return renderer.RenderView(relativePath, ViewModel);
        }

        public void View(string relativePath)
        {
            string result = LoadView(relativePath);
            WriteResponse(result);
        }

        public void Redirect(string relativePath)
        {
            Response.Redirect(relativePath);
        }

        protected void SetContentType(string mimeType)
        {
            ResponseType = ContentType.Other;
            ResponseMimeType = mimeType;
        }
    }
}
