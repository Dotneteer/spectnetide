using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the ERROR pragma
    /// </summary>
    public sealed class ErrorPragma : PragmaBase
    {
        /// <summary>
        /// The error string value
        /// </summary>
        public ExpressionNode Expr { get; }

        public ErrorPragma(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}