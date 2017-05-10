using System;

namespace Handlr.Framework.Web.Attributes
{
    public sealed class MapsTo : Attribute
    {
        public MapsTo(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
