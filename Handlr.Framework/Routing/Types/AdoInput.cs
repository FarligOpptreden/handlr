using System.Collections.Generic;
using System.Data;
using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents a results of an ADO call that can be passed to a translator.
    /// </summary>
    public class AdoInput : IInput
    {
        /// <summary>
        /// Gets the list of Data Tables associated with the input.
        /// </summary>
        public List<DataTable> Data { get; private set; }

        public AdoInput(List<DataTable> data)
        {
            Data = data;
        }
    }
}
