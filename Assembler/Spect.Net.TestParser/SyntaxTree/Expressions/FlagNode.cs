using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents an identifier as an expression
    /// </summary>
    public sealed class FlagNode : ExpressionNode
    {
        /// <summary>
        /// Creates an expression node with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public FlagNode(ParserRuleContext context) : base(context)
        {
            FlagName = context.GetText().Replace(".", "").Replace(" ", "").ToLower();
        }

        /// <summary>
        /// The name of the flag
        /// </summary>
        public string FlagName { get; set; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IExpressionEvaluationContext evalContext)
            => evalContext.IsMachineAvailable();

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            var flagValue = evalContext.GetFlagValue(FlagName);
            return  new ExpressionValue(flagValue);
        }
    }
}