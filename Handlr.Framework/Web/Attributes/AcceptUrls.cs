using System;
using System.Linq;

namespace Handlr.Framework.Web.Attributes
{
    public sealed class AcceptUrls : Attribute
    {
        public AcceptUrls(params string[] patterns)
        {
            Patterns = patterns.Select(p => p.ToLower()).ToArray();
        }

        public string[] Patterns { get; private set; }
    }
}
