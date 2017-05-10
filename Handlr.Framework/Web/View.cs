using Handlr.Framework.Web.ViewModel;

namespace Handlr.Framework.Web
{
    public abstract class View<T> : System.Web.Mvc.WebViewPage<Container>
    {
        public Application Application
        {
            get
            {
                return Model.Application;
            }
        }

        public Config Configuration
        {
            get
            {
                return Model.Configuration;
            }
        }

        public Validation Validation
        {
            get
            {
                return Model.Validation;
            }
        }

        public View ViewDetails
        {
            get
            {
                return Model.View;
            }
        }

        public T ViewModel
        {
            get
            {
                return Model.GetModel<T>();
            }
        }

        public P GetProperty<P>(string key)
        {
            return Model.GetProperty<P>(key);
        }
    }
}
