namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This interface defines a context in which expressions are evaluated.
    /// </summary>
    public interface IExpressionEvaluationContext
    {
        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        ExpressionValue GetSymbolValue(string symbol);

        /// <summary>
        /// Gets the current value of the specified Z80 register
        /// </summary>
        /// <param name="registerName">Name of the register</param>
        /// <param name="is8Bit">Is it an 8-bit register?</param>
        /// <returns>Z80 register value</returns>
        ExpressionValue GetZ80RegisterValue(string registerName, out bool is8Bit);

        /// <summary>
        /// Gets the current value of the specified Z80 flag
        /// </summary>
        /// <param name="flagName">Name of the flag</param>
        /// <returns>Z80 register value</returns>
        ExpressionValue GetZ80FlagValue(string flagName);

        /// <summary>
        /// Gets the current value of the memory pointed by the specified Z80 register
        /// </summary>
        /// <param name="registerName">Name of the register</param>
        /// <returns>Z80 register value</returns>
        ExpressionValue GetZ80RegisterIndirectValue(string registerName);

        /// <summary>
        /// Gets the current value of the memory pointed by the specified Z80 register
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <returns>Z80 register value</returns>
        ExpressionValue GetMemoryIndirectValue(ExpressionValue address);
    }
}