using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFG pragma
    /// </summary>
    public sealed class DefgPragma : PragmaBase
    {
        /// <summary>
        /// The DEFG pattern value
        /// </summary>
        public ExpressionNode Expr { get; }

        public DefgPragma(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}