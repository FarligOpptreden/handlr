using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Handlr.Framework;
using Handlr.Framework.Routing.Interfaces;
using System.Collections;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents the output from a REST process.
    /// </summary>
    public class RestOutput : IOutput
    {
        private object Model;

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
        /// <typeparam name="D">The type of the resulting data</typeparam>
        /// <typeparam name="M">The type of the data used in the view</typeparam>
        /// <param name="data">The data resulting from the process' execution</param>
        /// <param name="model">The data used in the view</param>
        public void SetData<D, M>(D data, M model)
        {
            Data = data;
            Model = model;
        }

        public T GetModel<T>()
        {
            return (T)Model;
        }

        public dynamic GetDynamicModel()
        {
            return (Model as IDictionary<string, object>).ToDynamic();
        }
    }
}
