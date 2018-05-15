using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a LOOP statement
    /// </summary>
    public sealed class UntilStatement : EndStatementBase
    {
        /// <summary>
        /// Until expression
        /// </summary>
        public ExpressionNode Expr { get; }

        public UntilStatement(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}