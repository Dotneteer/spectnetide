namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This interface represents the contex an expression
    /// is evaluated in
    /// </summary>
    public interface IEvaluationContext
    {
        /// <summary>
        /// Gets the current assembly address
        /// </summary>
        ushort GetCurrentAddress();

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        ushort? GetSymbolValue(string symbol);
    }
}