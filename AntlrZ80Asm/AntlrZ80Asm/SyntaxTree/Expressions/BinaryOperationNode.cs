namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an abstract binary operation
    /// </summary>
    public abstract class BinaryOperationNode : ExpressionNode
    {
        /// <summary>
        /// Left operand
        /// </summary>
        public ExpressionNode LeftOperand { get; set; }

        /// <summary>
        /// Right operand
        /// </summary>
        public ExpressionNode RightOperand { get; set; }

        /// <summary>
        /// Retrieves any error in child operator nodes
        /// </summary>
        public override string EvaluationError
            => LeftOperand.EvaluationError ?? RightOperand.EvaluationError;

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <returns>Evaluated expression value</returns>
        public override ushort Evaluate()
        {
            return EvaluationError == null ? Calculate() : (ushort)0;
        }

        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <returns>Result of the operation</returns>
        public abstract ushort Calculate();
    }
}