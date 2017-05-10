using System;

namespace Handlr.Framework.Web
{
    public static class Helpers
    {
        public static bool IsAbsolute(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }
    }
}
