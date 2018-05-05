namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents the current loop counter value
    /// </summary>
    public sealed class CurrentLoopCounterNode : ExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            return new ExpressionValue(evalContext.GetLoopCounterValue());
        }
    }
}