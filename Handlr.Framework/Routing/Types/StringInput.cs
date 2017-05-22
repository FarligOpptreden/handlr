using Handlr.Framework.Routing.Interfaces;

namespace Handlr.Framework.Routing.Types
{
    /// <summary>
    /// Represents a standard string input that can be passed to a translator.
    /// </summary>
    public class StringInput : IInput
    {
        /// <summary>
        /// Gets the string representation of the input.
        /// </summary>
        public string Input { get; private set; }

        public StringInput(string input)
        {
            Input = input;
        }
    }
}
