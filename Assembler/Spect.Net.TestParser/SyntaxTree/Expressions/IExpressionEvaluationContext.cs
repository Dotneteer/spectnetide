namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This interface represents the contex an expression
    /// is evaluated in
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
        /// Gets the flag that indicates if machine is available
        /// </summary>
        /// <returns></returns>
        bool IsMachineAvailable();

        /// <summary>
        /// Gets the value of the specified Z80 register
        /// </summary>
        /// <param name="regName">Register name</param>
        /// <returns>
        /// The register's current value
        /// </returns>
        ushort GetRegisterValue(string regName);

        /// <summary>
        /// Gets the value of the specified Z80 flag
        /// </summary>
        /// <param name="flagName">Register name</param>
        /// <returns>
        /// The flags's current value
        /// </returns>
        bool GetFlagValue(string flagName);

        /// <summary>
        /// Gets the range of the machines memory from start to end
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        byte[] GetMemorySection(ushort start, ushort end);

        /// <summary>
        /// Gets the range of memory reach values
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        byte[] GetReachSection(ushort start, ushort end);
    }
}