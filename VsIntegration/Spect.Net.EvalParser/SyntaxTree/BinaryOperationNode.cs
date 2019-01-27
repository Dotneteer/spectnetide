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

        /// <summary>
        /// Suggests the expression type to be one size larger than the wider operand
        /// </summary>
        protected void SuggestWidestType()
        {
            if (LeftOperand.ValueType == ExpressionValueType.DWord
                || LeftOperand.ValueType == ExpressionValueType.Word
                || RightOperand.ValueType == ExpressionValueType.DWord
                || RightOperand.ValueType == ExpressionValueType.Word)
            {
                ValueType = ExpressionValueType.DWord;
            }
            else
            {
                ValueType = ExpressionValueType.Word;
            }
        }

        /// <summary>
        /// Suggests the expression type to be the size of the wider operand
        /// </summary>
        protected void SuggestWiderType()
        {
            if (LeftOperand.ValueType == ExpressionValueType.DWord
                || RightOperand.ValueType == ExpressionValueType.DWord)
            {
                ValueType = ExpressionValueType.DWord;
            }
            else if (LeftOperand.ValueType == ExpressionValueType.Word
                || RightOperand.ValueType == ExpressionValueType.Word)
            {
                ValueType = ExpressionValueType.Word;
            }
            else
            {
                ValueType = ExpressionValueType.Byte;
            }
        }
    }
}