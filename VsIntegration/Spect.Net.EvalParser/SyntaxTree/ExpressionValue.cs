namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents the value of an evaluated expression
    /// </summary>
    public class ExpressionValue
    {
        /// <summary>
        /// Used in case of expression evaluation errors
        /// </summary>
        public static ExpressionValue Error = new ExpressionValue();

        /// <summary>
        /// Checks if the value of this expression is valid
        /// </summary>
        public bool IsValid => this != Error;

        /// <summary>
        /// Initializes a new instance of the class with the Error value.
        /// </summary>
        private ExpressionValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified uint value.
        /// </summary>
        /// <param name="value">Uint value</param>
        public ExpressionValue(uint value)
        {
            Value = value;
        }

        /// <summary>
        /// Shortcut property to get the 16-bit value behind the expression
        /// </summary>
        public uint Value { get; }

        /// <summary>
        /// Implicit operator to ushort
        /// </summary>
        /// <param name="value">Value to convert</param>
        public static implicit operator uint(ExpressionValue value) => value.Value;
    }
}