using System;

namespace Handlr.Framework.Web.Attributes
{
    public sealed class Minify : Attribute
    {
        public Minify(bool minify = true)
        {
            DoMinification = minify;
        }

        public bool DoMinification { get; private set; }
    }
}
