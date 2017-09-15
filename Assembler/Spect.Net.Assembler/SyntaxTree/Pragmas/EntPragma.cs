using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the ENT pragma
    /// </summary>
    public sealed class EntPragma : PragmaBase
    {
        /// <summary>
        /// The ENT parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}