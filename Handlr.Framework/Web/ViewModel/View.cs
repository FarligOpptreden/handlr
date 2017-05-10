using System.Collections.Generic;

namespace Handlr.Framework.Web.ViewModel
{
    public class View
    {
        internal View()
        {
            CssIncludes = new List<string>();
            ExternalCssIncludes = new List<string>();
            ScriptIncludes = new List<string>();
            ExternalScriptIncludes = new List<string>();
            Meta = new Dictionary<string, string>();
            Keywords = new List<string>();
        }

        public string Title { get; set; }
        public string Layout { get; set; }
        public string BundleKey { get; set; }
        public List<string> CssIncludes { get; private set; }
        public List<string> ExternalCssIncludes { get; private set; }
        public List<string> ScriptIncludes { get; private set; }
        public List<string> ExternalScriptIncludes { get; private set; }
        public Dictionary<string, string> Meta { get; private set; }
        public List<string> Keywords { get; private set; }
    }
}
