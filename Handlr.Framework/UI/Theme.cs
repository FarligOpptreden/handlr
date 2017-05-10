using Handlr.Framework.Web;
using Handlr.Framework.Web.Attributes;

namespace Handlr.Framework.UI
{
    public class Theme : Handler
    {
        [AcceptUrls("/svg/[a-zA-Z\\-\\s]+")]
        [CacheOutput]
        [Minify(false)]
        string GetSvg(string colour)
        {
            string icon = UriSegments[1];
            string path = Server.MapPath(Request.ApplicationPath + "/images/icons/" + icon + ".svg");
            if (!IO.IsFile(path))
                return null;
            string svg = IO.ReadTextFile(path);
            SetContentType("image/svg+xml");
            return svg.Replace("#000000", "#" + colour);
        }
    }
}
