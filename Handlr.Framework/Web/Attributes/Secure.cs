using System;
using System.Web;

namespace Handlr.Framework.Web.Attributes
{
    public sealed class Secure : Attribute
    {
        public Secure(bool isSecure = true, bool nonceCheck = true, string redirectUrl = null)
        {
            IsSecure = isSecure;
            NonceCheck = nonceCheck;
            RedirectUrl = !string.IsNullOrEmpty(redirectUrl) ? redirectUrl.ToLower() : HttpContext.Current.Request.Url.AbsoluteUri;
        }

        public bool IsSecure { get; private set; }

        public string RedirectUrl { get; private set; }

        public bool NonceCheck { get; private set; }
    }
}
