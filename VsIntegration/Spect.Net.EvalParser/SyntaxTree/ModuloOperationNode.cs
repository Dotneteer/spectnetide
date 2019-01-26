namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents the add operation
    /// </summary>
    public sealed class ModuloOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public override ExpressionValue Calculate(IExpressionEvaluationContext evalContext)
        {
            var left = LeftOperand.Evaluate(evalContext);
            var right = RightOperand.Evaluate(evalContext);
            if (!left.IsValid || !right.IsValid)
            {
                return ExpressionValue.Error;
            } 
            if (right.Value != 0)
            {
                return new ExpressionValue(left.Value % right.Value);
            }
            EvaluationError = "Divide by zero error";
            return ExpressionValue.Error;
        }
    }
}