namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an UNARY logical NOT operation
    /// </summary>
    public sealed class UnaryLogicalNotNode : UnaryExpressionNode
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
                    EvaluationError = "Unary logical NOT operator cannot be applied on a byte array";
                    return ExpressionValue.Error;
                case ExpressionValueType.Bool:
                case ExpressionValueType.Byte:
                case ExpressionValueType.Word:
                    return new ExpressionValue(!operandValue.AsBool());
                default:
                    return ExpressionValue.Error;
            }
        }
    }
}