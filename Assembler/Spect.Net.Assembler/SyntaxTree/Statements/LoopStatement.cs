using System;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a LOOP statement
    /// </summary>
    public sealed class LoopStatement : BlockStatementBase
    {
        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(LoopEndStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "ENDM/MEND";

        /// <summary>
        /// Loop expression
        /// </summary>
        public ExpressionNode Expr { get; }

        public LoopStatement(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}