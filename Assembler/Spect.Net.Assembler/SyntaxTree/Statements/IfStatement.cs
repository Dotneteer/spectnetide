using System;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents an IF statement
    /// </summary>
    public class IfStatement : BlockStatementBase
    {
        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(IfEndStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "ENDIF";

        /// <summary>
        /// IF expression
        /// </summary>
        public ExpressionNode Expr { get; }

        public IfStatement(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}