using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// Represents an expression node that can be evaluated
    /// </summary>
    public abstract class ExpressionNode: NodeBase
    {
        /// <summary>
        /// Creates an expression node with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        protected ExpressionNode(ParserRuleContext context) : base(context)
        {
        }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public virtual bool ReadyToEvaluate(IExpressionEvaluationContext evalContext) => true;

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public abstract ExpressionValue Evaluate(IExpressionEvaluationContext evalContext);

        /// <summary>
        /// Retrieves the evaluation error text, provided there is any issue
        /// </summary>
        public virtual string EvaluationError { get; set; } = null;
    }
}