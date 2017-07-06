using Handlr.Framework.Web;
using Handlr.Framework.Web.ViewModel;
using System.Web;

namespace Handlr.Framework.UI
{
    /// <summary>
    /// Exposes methods to render full and partial views.
    /// </summary>
    public static class Views
    {
        /// <summary>
        /// Renders a partial view with the specified model as its view model.
        /// </summary>
        /// <param name="handler">The execution context used to render the view</param>
        /// <param name="relativePath">The relative path of the view to load</param>
        /// <param name="model">The object to use as view model</param>
        /// <param name="viewModel">The view model used to render the partial view</param>
        /// <returns>The rendered partial view</returns>
        public static IHtmlString Partial(Handler handler, string relativePath, object model, out Container viewModel)
        {
            if (relativePath.IndexOf(".cshtml") < 0 && relativePath.IndexOf(".cshtml") <= 0)
                relativePath += ".cshtml";
            ViewRenderer renderer = new ViewRenderer();
            viewModel = new Container(handler.Configuration, handler.AppPath, handler.Context, handler);
            viewModel.SetModel(model);
            return new HtmlString(renderer.RenderView(relativePath, viewModel));
        }

        /// <summary>
        /// Renders a partial view with the specified model as its view model.
        /// </summary>
        /// <param name="handler">The execution context used to render the view</param>
        /// <param name="relativePath">The relative path of the view to load</param>
        /// <param name="model">The object to use as view model</param>
        /// <returns>The rendered partial view</returns>
        public static IHtmlString Partial(Handler handler, string relativePath, object model)
        {
            if (relativePath.IndexOf(".cshtml") < 0 && relativePath.IndexOf(".cshtml") <= 0)
                relativePath += ".cshtml";
            ViewRenderer renderer = new ViewRenderer();
            var viewModel = new Container(handler.Configuration, handler.AppPath, handler.Context, handler);
            viewModel.SetModel(model);
            return new HtmlString(renderer.RenderView(relativePath, viewModel));
        }
    }
}
