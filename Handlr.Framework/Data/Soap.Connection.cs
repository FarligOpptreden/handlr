using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Data.Soap
{
    public sealed class Connection : Http.Connection
    {
        public Connection(string url) :
            base(url, Method.Post)
        {
            ContentType = "application/soap+xml; charset=utf-8";
        }
    }
}
