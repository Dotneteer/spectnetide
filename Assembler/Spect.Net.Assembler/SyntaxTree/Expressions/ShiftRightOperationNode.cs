namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the shift rightí operation
    /// </summary>
    public sealed class ShiftRightOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public override ExpressionValue Calculate(IEvaluationContext evalContext)
        {
            var left = LeftOperand.Evaluate(evalContext);
            var right = RightOperand.Evaluate(evalContext);
            if (left.Type != ExpressionValueType.Bool && left.Type != ExpressionValueType.Integer)
            {
                EvaluationError = "The left operand of the shift right operator must be integral";
            }
            if (right.Type != ExpressionValueType.Bool && right.Type != ExpressionValueType.Integer)
            {
                EvaluationError = "Right operand of the shift right operator must be integral";
            }
            return new ExpressionValue(left.AsLong() >> (ushort)right.AsLong());
        }
    }
}