using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Pragmas
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