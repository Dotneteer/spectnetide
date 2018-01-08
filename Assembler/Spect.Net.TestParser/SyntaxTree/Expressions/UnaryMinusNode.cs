namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an UNARY - operation
    /// </summary>
    public sealed class UnaryMinusNode : UnaryExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            // --- Check operand error
            var operandValue = Operand.Evaluate(evalContext);
            if (operandValue.Type == ExpressionValueType.Error)
            {
                EvaluationError = Operand.EvaluationError;
                return ExpressionValue.Error;
            }

            // --- Carry out operation
            switch (operandValue.Type)
            {
                case ExpressionValueType.ByteArray:
                    EvaluationError = "Unary minus operator cannot be applied on a byte array";
                    return ExpressionValue.Error;
                case ExpressionValueType.Bool:
                case ExpressionValueType.Byte:
                case ExpressionValueType.Word:
                    return new ExpressionValue((ushort)-operandValue.AsWord());
                default:
                    return ExpressionValue.Error;
            }
        }
    }
}