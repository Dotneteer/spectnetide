namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents the current assembly address
    /// </summary>
    public sealed class CurrentAddressNode : ExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ushort Evaluate(IEvaluationContext evalContext)
        {
            return evalContext.GetCurrentAddress();
        }
    }
}