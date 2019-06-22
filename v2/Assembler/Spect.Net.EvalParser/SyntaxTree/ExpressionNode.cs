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
        /// The value type of the evaluated expression
        /// </summary>
        public ExpressionValueType ValueType { get; protected set; }

        /// <summary>
        /// Retrieves the evaluation error text, provided there is any issue
        /// </summary>
        public virtual string EvaluationError { get; set; } = null;

        /// <summary>
        /// Sets the value type of this expression to the one specified in the argument.
        /// </summary>
        /// <param name="node">Expression node to take type information from</param>
        protected void SuggestTypeOf(ExpressionNode node)
        {
            if (node != null)
            {
                ValueType = node.ValueType;
            }
        }

        /// <summary>
        /// Sets the value type of this expression to the one specified in the arument.
        /// </summary>
        /// <param name="type">Value type to set</param>
        protected void SuggestType(ExpressionValueType type)
        {
            ValueType = type;
        }
    }
}