using Handlr.Framework.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Handlr.Framework.UI.Types
{
    public class ViewModelResponse<T>
    {
        private Handler _ControllerContext;
        private string _PageIdentifier;
        private string[] _ExternalScriptIncludes;
        private List<string> _ScriptIncludes;
        private string[] _ExternalCssIncludes;
        private List<string> _CssIncludes;

        private void ctor(Handler controllerContext, string pageIdentifier, string view)
        {
            _ControllerContext = controllerContext;
            _ControllerContext.ViewModel.View.CssIncludes.Clear();
            _ControllerContext.ViewModel.View.ExternalCssIncludes.Clear();
            _ControllerContext.ViewModel.View.ScriptIncludes.Clear();
            _ControllerContext.ViewModel.View.ExternalScriptIncludes.Clear();
            _PageIdentifier = pageIdentifier;
            Markup = _ControllerContext.LoadView(view);
            _ScriptIncludes = _ControllerContext.ViewModel.View.ScriptIncludes;
            _ExternalScriptIncludes = new string[_ControllerContext.ViewModel.View.ExternalScriptIncludes.Count];
            _ControllerContext.ViewModel.View.ExternalScriptIncludes.CopyTo(_ExternalScriptIncludes);
            _CssIncludes = _ControllerContext.ViewModel.View.CssIncludes;
            _ExternalCssIncludes = new string[_ControllerContext.ViewModel.View.ExternalCssIncludes.Count];
            _ControllerContext.ViewModel.View.ExternalCssIncludes.CopyTo(_ExternalCssIncludes);
        }

        public ViewModelResponse(Handler controllerContext, string pageIdentifier, string view, T model)
        {
            controllerContext.ViewModel.SetModel(model);
            ctor(controllerContext, pageIdentifier, view);
        }

        public ViewModelResponse(Handler controllerContext, string pageIdentifier, string view, T model, bool minifyMarkup)
        {
            controllerContext.ViewModel.SetModel(model);
            ctor(controllerContext, pageIdentifier, view);
            if (minifyMarkup)
                Markup = _ControllerContext.Minify(Markup);
        }

        public ViewModelResponse(Handler controllerContext, string pageIdentifier, string view, bool minifyMarkup)
        {
            ctor(controllerContext, pageIdentifier, view);
            if (minifyMarkup)
                Markup = _ControllerContext.Minify(Markup);
        }

        public ViewModelResponse(Handler controllerContext, string pageIdentifier, string view)
        {
            ctor(controllerContext, pageIdentifier, view);
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ScriptBundle
        {
            get
            {
                if (_ScriptIncludes == null || _ScriptIncludes.Count == 0)
                    return null;
                return Bundler.Bundle(_ScriptIncludes, Bundler.Type.Js, _PageIdentifier);
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ExternalScriptIncludes
        {
            get
            {
                if (_ExternalScriptIncludes == null || _ExternalScriptIncludes.Count() == 0)
                    return null;
                return _ExternalScriptIncludes.ToList();
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CssBundle
        {
            get
            {
                if (_CssIncludes == null || _CssIncludes.Count == 0)
                    return null;
                return Bundler.Bundle(_CssIncludes, Bundler.Type.Css, _PageIdentifier);
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ExternalCssIncludes
        {
            get
            {
                if (_ExternalCssIncludes == null || _ExternalCssIncludes.Count() == 0)
                    return null;
                return _ExternalCssIncludes.ToList();
            }
        }

        public string Markup { get; private set; }

        public T ViewModel
        {
            get
            {
                return _ControllerContext.ViewModel.GetModel<T>();
            }
        }
    }
}
