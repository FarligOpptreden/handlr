using System.Collections.Generic;
using System.Web;

namespace Handlr.Framework.Web.ViewModel
{
    public class Container
    {
        private object _Model;
        private Dictionary<string, object> _Properties;

        internal Container(Config configuration, string appRoot)
        {
            Application = new Application(configuration, appRoot);
            Configuration = configuration;
            Validation = new Validation();
            View = new View();
            _Properties = new Dictionary<string, object>();
        }

        internal Container(Config configuration, string appRoot, HttpContext httpContext)
        {
            Application = new Application(configuration, appRoot, httpContext);
            Configuration = configuration;
            Validation = new Validation();
            View = new View();
            _Properties = new Dictionary<string, object>();
        }

        internal Container(Config configuration, string appRoot, HttpContext httpContext, Handler controllerContext)
        {
            Application = new Application(configuration, appRoot, httpContext);
            Configuration = configuration;
            Validation = new Validation();
            View = new View();
            ControllerContext = controllerContext;
            _Properties = new Dictionary<string, object>();
        }

        public Application Application { get; private set; }
        public Validation Validation { get; private set; }
        public View View { get; private set; }
        public Config Configuration { get; private set; }
        public Handler ControllerContext { get; private set; }
        
        public T GetModel<T>()
        {
            return (T)_Model;
        }

        public void SetModel<T>(T model)
        {
            _Model = model;
        }

        public void SetProperty<T>(string key, T value)
        {
            if (!_Properties.ContainsKey(key))
                _Properties.Add(key, value);
            else
                _Properties[key] = value;
        }

        public T GetProperty<T>(string key)
        {
            if (!_Properties.ContainsKey(key))
                return default(T);
            if (_Properties[key].GetType() != typeof(T))
                return default(T);
            return (T)_Properties[key];
        }
    }
}
