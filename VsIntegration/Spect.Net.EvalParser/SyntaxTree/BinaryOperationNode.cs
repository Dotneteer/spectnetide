namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents an abstract binary operation
    /// </summary>
    public abstract class BinaryOperationNode : ExpressionNode
    {
        private string _evalError;

        /// <summary>
        /// Left operand
        /// </summary>
        public ExpressionNode LeftOperand { get; set; }

        /// <summary>
        /// Right operand
        /// </summary>
        public ExpressionNode RightOperand { get; set; }

        /// <summary>
        /// Retrieves any error in child operator nodes
        /// </summary>
        public override string EvaluationError
        {
            get => _evalError ?? LeftOperand.EvaluationError ?? RightOperand.EvaluationError;
            set => _evalError = value;
        }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            return EvaluationError == null ? Calculate(evalContext) : ExpressionValue.Error;
        }

        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public abstract ExpressionValue Calculate(IExpressionEvaluationContext evalContext);
    }
}