using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the DISP pragma
    /// </summary>
    public sealed class DispPragma : PragmaBase
    {
        /// <summary>
        /// The DISP parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}