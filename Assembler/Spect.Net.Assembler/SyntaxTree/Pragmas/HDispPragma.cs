using Spect.Net.Assembler.SyntaxTree.Expressions;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace Spect.Net.Assembler.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the HDISP pragma
    /// </summary>
    public sealed class HDispPragma : PragmaBase
    {
        /// <summary>
        /// The HDISP parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}