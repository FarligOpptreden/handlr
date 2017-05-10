using Handlr.Framework.Web.Interfaces;
using System;

namespace Handlr.Framework.Web.Types
{
    public class DefaultNonceProvider : INonceProvider
    {
        public IController ControllerContext { get; set; }

        public void StoreNonce(string userId, string nonce)
        {
            return;
        }

        public bool ValidateNonce(string userId, string nonce)
        {
            return true;
        }

        public void DeleteNonce(string nonce)
        {
            return;
        }
    }
}
