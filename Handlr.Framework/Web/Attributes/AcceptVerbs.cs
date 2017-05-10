using System;
using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Web.Attributes
{
    public sealed class AcceptVerbs : Attribute
    {
        public AcceptVerbs(params Method[] verbs)
        {
            Verbs = verbs;
        }

        public Method[] Verbs { get; private set; }
    }
}
