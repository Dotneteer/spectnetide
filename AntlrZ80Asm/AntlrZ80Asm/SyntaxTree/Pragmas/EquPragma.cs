using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree.Pragmas
{
    /// <summary>
    /// This class represents the EQU pragma
    /// </summary>
    public sealed class EquPragma : PragmaLine
    {
        /// <summary>
        /// The EQU parameter
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}