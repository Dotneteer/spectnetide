namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an abstract unary operation
    /// </summary>
    public abstract class UnaryExpressionNode : ExpressionNode
    {
        /// <summary>
        /// Operand of the unary operation
        /// </summary>
        public ExpressionNode Operand { get; set; }
    }
}