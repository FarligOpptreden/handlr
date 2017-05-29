using Handlr.Framework.Routing.Exceptions;
using System;

namespace Handlr.Framework.Routing.Types
{
    public class ClrParameter
    {
        public Type Type { get; set; }

        public string ValueKey { get; set; }

        public ClrParameter() { }

        public ClrParameter(string typeName, string valueKey)
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException("typeName");
            Type = Type.GetType(typeName);
            if (Type == null)
                throw new ParserException(string.Format("The specified type \"{0}\" could not be evaluated", typeName));
            ValueKey = valueKey;
        }
    }
}
