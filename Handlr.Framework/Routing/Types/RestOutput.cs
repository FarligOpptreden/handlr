using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents the output from a REST process.
    /// </summary>
    public class RestOutput : IOutput
    {
        /// <summary>
        /// Gets a list of exceptions that occurred during the execution of the process.
        /// </summary>
        [JsonIgnore]
        public List<Exception> Exceptions { get; private set; } = new List<Exception>();

        /// <summary>
        /// Gets a value indicating whether the process executed successfully or not.
        /// </summary>
        public bool Success { get { return Exceptions.Count == 0; } }

        /// <summary>
        /// Gets an array of errors based on the exceptions that occurred during the execution of the process.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Errors
        {
            get
            {
                if (Exceptions.Count == 0)
                    return null;
                return
                    (
                        from e in Exceptions
                        select e.Message
                    ).ToArray();
            }
        }

        /// <summary>
        /// Gets the data returned by the process.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; private set; }

        /// <summary>
        /// Sets the data for the result of the process.
        /// </summary>
        /// <typeparam name="T">The type of the resulting data</typeparam>
        /// <param name="data">The data resulting from the process' execution</param>
        public void SetData<T>(T data)
        {
            Data = data;
        }
    }
}
