using System;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// Represents a block statement with the specified end statement
    /// </summary>
    /// <typeparam name="TEnd"></typeparam>
    public abstract class BlockStatementBase<TEnd> : StatementBase
        where TEnd : EndStatementBase
    {
        /// <summary>
        /// Type of end statement
        /// </summary>
        public Type EndType => typeof(TEnd);
    }
}