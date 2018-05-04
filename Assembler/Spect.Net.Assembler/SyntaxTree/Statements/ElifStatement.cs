using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents an ELIF statement
    /// </summary>
    public sealed class ElifStatement : StatementBase
    {
        /// <summary>
        /// IF expression
        /// </summary>
        public ExpressionNode Expr { get; }

        public ElifStatement(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}