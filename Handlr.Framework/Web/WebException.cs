using System;
using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Web
{
    /// <summary>
    /// Represents a web exception that happened when handling a controller.
    /// </summary>
    public class WebException : Exception
    {
        private Status _Status;

        public WebException(Status status, string message, Exception innerException)
            : base(message, innerException)
        {
            _Status = status;
        }

        public WebException(Status status, string message)
            : this(status, message, null)
        { }

        /// <summary>
        /// Gets the status code of the exception.
        /// </summary>
        public Status Status
        {
            get
            {
                return _Status;
            }
        }

        /// <summary>
        /// Gets the stack trace of the exception.
        /// </summary>
        public new string StackTrace
        {
            get
            {
                if (InnerException != null)
                    return InnerException.StackTrace;
                return null;
            }
        }
    }
}
