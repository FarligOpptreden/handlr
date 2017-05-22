using System.Collections.Generic;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents a JSON construct that can be passed to a translator.
    /// </summary>
    public class JsonInput : StringInput
    {
        /// <summary>
        /// Gets a key-value representation of the JSON construct.
        /// </summary>
        public Dictionary<string, object> Json { get; private set; }

        public JsonInput(string json) : base(json)
        {
            Json = json.ToDictionary();
        }
    }
}
