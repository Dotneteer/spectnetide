using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the RNDSEED pragma
    /// </summary>
    public sealed class RndSeedPragma : PragmaBase
    {
        /// <summary>
        /// The optional seed value
        /// </summary>
        public ExpressionNode Expr { get; }

        public RndSeedPragma(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}