using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFGX pragma
    /// </summary>
    public sealed class DefgxPragma : PragmaBase
    {
        /// <summary>
        /// The DEFGX pattern expression
        /// </summary>
        public ExpressionNode Expr { get; }

        public DefgxPragma(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}