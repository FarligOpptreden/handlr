namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents an ADO parameter to be parsed and passed to a query for execution.
    /// </summary>
    public class AdoParameter
    {
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the data type of the parameter.
        /// </summary>
        public DataType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is nullable or not.
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// The data type of a parameter.
        /// </summary>
        public enum DataType
        {
            Int,
            Float,
            String,
            DateTime,
            Bit
        }
    }
}
