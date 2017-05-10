using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Data.Http
{
    public sealed class Delete : Connector
    {
        private Delete(string url)
            : base(url)
        {
        }

        public override void Initialize()
        {
            Connection = new Connection(ConnectionString, Method.Delete);
        }
    }
}
