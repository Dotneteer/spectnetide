namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents a conditional (?:) operation
    /// </summary>
    public class ConditionalExpressionNode : ExpressionNode
    {
        /// <summary>
        /// Condition of the expression
        /// </summary>
        public ExpressionNode Condition { get; set; }

        /// <summary>
        /// Value when the expression is true
        /// </summary>
        public ExpressionNode TrueExpression { get; set; }

        /// <summary>
        /// Value when the expression is false
        /// </summary>
        public ExpressionNode FalseExpression { get; set; }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            var cond = Condition.Evaluate(evalContext);
            if (!cond.IsValid)
            {
                return ExpressionValue.Error;
            }

            if (cond.Value != 0)
            {
                var result = TrueExpression.Evaluate(evalContext);
                SuggestTypeOf(TrueExpression);
                return result;
            }
            else
            {
                var result = FalseExpression.Evaluate(evalContext);
                SuggestTypeOf(FalseExpression);
                return result;
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => $"{Condition} ? {TrueExpression} : {FalseExpression}";
    }
}