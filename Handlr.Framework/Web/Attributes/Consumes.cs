using System;
using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Web.Attributes
{
    public sealed class Consumes : Attribute
    {
        public Consumes(ContentType responseType)
        {
            Type = responseType;
        }

        public ContentType Type { get; private set; }
    }
}
