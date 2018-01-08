namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents an identifier as an expression
    /// </summary>
    public sealed class RegisterNode : ExpressionNode
    {
        /// <summary>
        /// The name of the Z80 register
        /// </summary>
        public string RegisterName { get; set; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IEvaluationContext evalContext)
            => evalContext.IsMachineAvailable();

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            var regValue = evalContext.GetRegisterValue(RegisterName);
            return new ExpressionValue(regValue);
        }
    }
}