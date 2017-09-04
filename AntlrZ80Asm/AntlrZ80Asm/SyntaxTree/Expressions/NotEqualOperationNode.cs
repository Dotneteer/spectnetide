namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the 'not equal' operation
    /// </summary>
    public sealed class NotEqualOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate(IEvaluationContext evalContext)
            => (ushort)(LeftOperand.Evaluate(evalContext) != RightOperand.Evaluate(evalContext) ? 1 : 0);
    }
}