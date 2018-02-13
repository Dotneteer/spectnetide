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
        /// Gets the machine context to evaluate registers, flags, and memory
        /// </summary>
        /// <returns></returns>
        IMachineContext GetMachineContext();
    }
}