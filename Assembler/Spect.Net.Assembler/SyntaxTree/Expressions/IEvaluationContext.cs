using System.Collections.Generic;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This interface represents the context an expression
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
        /// <param name="startFromGlobal">Should resolution start from global scope?</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        (ExpressionValue ExprValue, IHasUsageInfo UsageInfo) GetSymbolValue(string symbol, bool startFromGlobal = false);

        /// <summary>
        /// Gets the current loop counter value
        /// </summary>
        ExpressionValue GetLoopCounterValue();
    }
}