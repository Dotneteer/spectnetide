using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the ALIGN pragma
    /// </summary>
    public sealed class AlignPragma : PragmaBase
    {
        /// <summary>
        /// The alignment value
        /// </summary>
        public ExpressionNode Expr { get; }

        public AlignPragma(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}