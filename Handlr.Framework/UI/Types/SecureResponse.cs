using System.Collections.Generic;

namespace Handlr.Framework.UI.Types
{
    public class SecureResponse<T>
    {
        public SecureResponse(T data)
        {
            Data = data;
            Permissions = new Dictionary<string, bool>();
        }

        public T Data { get; private set; }

        public Dictionary<string, bool> Permissions { get; private set; }
    }
}
