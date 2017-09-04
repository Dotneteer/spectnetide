using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents a preprocessor directive
    /// </summary>
    public sealed class Directive : OperationBase
    {
        /// <summary>
        /// Optional identifier
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Optional expression
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}