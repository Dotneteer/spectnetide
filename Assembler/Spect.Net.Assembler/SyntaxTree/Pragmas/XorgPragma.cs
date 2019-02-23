using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the XORG pragma
    /// </summary>
    public sealed class XorgPragma : PragmaBase
    {
        /// <summary>
        /// The XORG parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}