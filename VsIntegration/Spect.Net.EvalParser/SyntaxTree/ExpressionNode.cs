namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// Represents an expression node that can be evaluated
    /// </summary>
    public abstract class ExpressionNode
    {
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