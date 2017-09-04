using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the SKIP pragma
    /// </summary>
    public sealed class SkipPragma : PragmaBase
    {
        /// <summary>
        /// The SKIP parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }

        /// <summary>
        /// The byte to fill the skipped memory
        /// </summary>
        public ExpressionNode Fill { get; set; }
    }
}