using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the XENT pragma
    /// </summary>
    public sealed class XentPragma : PragmaBase
    {
        /// <summary>
        /// The ENT parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}