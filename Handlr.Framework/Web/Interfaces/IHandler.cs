using System.Collections.Generic;
using System.Web;

namespace Handlr.Framework.Web.Interfaces
{
    public interface IHandler
    {
        void Init(HttpContext context, Config configuration);
        bool IsAuthenticated();
        void WriteResponse(object content, string contentType = null, int statusCode = 200, Dictionary<string, string> headers = null, bool minify = true);
        void WriteJsonResponse(IDictionary<string, object> content, bool minify = true);
    }
}
