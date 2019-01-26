namespace Spect.Net.EvalParser.SyntaxTree
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