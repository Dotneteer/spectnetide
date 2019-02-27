using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DEFGX pragma
    /// </summary>
    public sealed class DefgxPragma : PragmaBase, ISupportsFieldAssignment
    {
        /// <summary>
        /// The DEFGX pattern expression
        /// </summary>
        public ExpressionNode Expr { get; }

        /// <summary>
        /// True indicates that this node is used within a field assignment
        /// </summary>
        public bool IsFieldAssignment { get; set; }

        public DefgxPragma(ExpressionNode expr)
        {
            Expr = expr;
        }
    }
}