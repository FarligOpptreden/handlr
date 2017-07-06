using System;

namespace Handlr.Framework.Routing.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a translation could not be parsed.
    /// </summary>
    public class ParserException : Exception
    {
        public ParserException(string message) : base(message) { }

        public ParserException(string message, Exception innerException) : base(message, innerException) { }
    }
}
