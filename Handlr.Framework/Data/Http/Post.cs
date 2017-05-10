using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Data.Http
{
    public sealed class Post : Connector
    {
        private Post(string url)
            : base(url)
        {
        }

        public override void Initialize()
        {
            Connection = new Connection(ConnectionString, Method.Post);
        }
    }
}
