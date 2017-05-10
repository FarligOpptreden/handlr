using System;
using System.Linq;

namespace Handlr.Framework.Web.Attributes
{
    public sealed class Display : Attribute
    {
        public Display(params string[] values)
        {
            Values = values.Select(v => v.ToLower()).ToArray();
        }

        public string[] Values { get; private set; }
    }
}
