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

        /// <summary>
        /// Operator token
        /// </summary>
        public abstract string Operator { get; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"{Operator}{Operand}";
    }
}