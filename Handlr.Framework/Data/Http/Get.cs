using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Data.Http
{
    public sealed class Get : Connector
    {
        private Get(string url)
            : base(url)
        {
        }

        public override void Initialize()
        {
            Connection = new Connection(ConnectionString, Method.Get);
        }
    }
}
