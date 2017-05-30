using Handlr.Framework.Routing.Exceptions;
using System;

namespace Handlr.Framework.Routing.Types
{
    public class ClrParameter
    {
        public Type Type { get; set; }

        public string ValueKey { get; set; }

        public object Value { get; private set; }

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

        public object SetValue(object value)
        {
            Type valueType = Type.GetType(value.GetType().AssemblyQualifiedName);
            Type typeType = Type.GetType(Type.AssemblyQualifiedName);
            if (valueType != typeType && !typeType.IsAssignableFrom(valueType))
                throw new ParserException(string.Format("An object of type \"{0}\" is not compatible with the configured type \"{1}\"", value.GetType().ToString(), Type.ToString()));
            Value = value;
            return Value;
        }
    }
}
