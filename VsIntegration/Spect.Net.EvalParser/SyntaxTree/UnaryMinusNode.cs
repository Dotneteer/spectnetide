namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents an UNARY - operation
    /// </summary>
    public sealed class UnaryMinusNode : UnaryExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            var operand = Operand.Evaluate(evalContext);
            SuggestTypeOf(Operand);
            return new ExpressionValue((uint)-operand.Value);
        }
    }
}