using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Data.Http
{
    public sealed class Put : Connector
    {
        private Put(string url)
            : base(url)
        {
        }

        public override void Initialize()
        {
            Connection = new Connection(ConnectionString, Method.Put);
        }
    }
}
