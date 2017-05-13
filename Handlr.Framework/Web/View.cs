using Handlr.Framework.Web.ViewModel;

namespace Handlr.Framework.Web
{
    public abstract class View<T> : System.Web.Mvc.WebViewPage<Container>
    {
        public Handler ControllerContext
        {
            get => Model.ControllerContext;
        }

        public Application Application
        {
            get => Model.Application;
        }

        public Config Configuration
        {
            get => Model.Configuration;
        }

        public Validation Validation
        {
            get => Model.Validation;
        }

        public View ViewDetails
        {
            get => Model.View;
        }

        public T ViewModel
        {
            get => Model.GetModel<T>();
        }

        public P GetProperty<P>(string key)
        {
            return Model.GetProperty<P>(key);
        }
    }
}
