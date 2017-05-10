using System.Web;

namespace Handlr.Framework.Web.ViewModel
{
    public class Application
    {
        internal Application(Config configuration, string appRoot)
            : this(configuration, appRoot, HttpContext.Current)
        { }

        internal Application(Config configuration, string appRoot, HttpContext httpContext)
        {
            HttpRequest request = httpContext.Request;
            Root = appRoot != "/" ? appRoot : "";
            AccountsUrl = configuration.Accounts != null ? configuration.Accounts.Url + "?app=" + configuration.Application.Id + "&redirect=" + HttpUtility.UrlEncode(request.Url.AbsoluteUri) : null;
            Url = request.Url.Scheme + "://" + request.Url.DnsSafeHost + (request.Url.Port > 0 ? ":" + request.Url.Port : "") + appRoot;
        }

        public string Root { get; private set; }
        public string Url { get; private set; }
        public string AccountsUrl { get; private set; }
    }
}
