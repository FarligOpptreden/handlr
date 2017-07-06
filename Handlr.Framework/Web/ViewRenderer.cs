using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;

namespace Handlr.Framework.Web
{
    public class ViewRenderer
    {
        public ControllerContext Context { get; set; }

        public ViewRenderer()
        {
            Controller controller = new Controller();

            HttpContextBase wrapper = null;
            if (HttpContext.Current != null)
                wrapper = new HttpContextWrapper(HttpContext.Current);
            RouteData route = new RouteData();
            route.Values.Add("controller", "Handlr");
            controller.ControllerContext = new ControllerContext(wrapper, route, controller);
            Context = controller.ControllerContext;
        }

        public string RenderView(string viewPath, object model)
        {
            return RenderViewToStringInternal(viewPath, model, false);
        }

        public string RenderPartialView(string viewPath, object model)
        {
            return RenderViewToStringInternal(viewPath, model, true);
        }

        protected string RenderViewToStringInternal(string viewPath, object model, bool partial = false)
        {
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(Context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(Context, viewPath, null);

            var view = viewEngineResult.View;
            Context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter()) 
            {
                var ctx = new ViewContext(Context, view, Context.Controller.ViewData, Context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        private class Controller : System.Web.Mvc.Controller
        {
        }
    }
}