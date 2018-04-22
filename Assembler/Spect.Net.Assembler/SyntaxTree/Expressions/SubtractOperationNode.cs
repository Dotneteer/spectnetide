namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the subtract operation
    /// </summary>
    public sealed class SubtractOperationNode : BinaryOperationNode
    {
        private const string LEFT_STRING_ERROR = "The left operand of subtraction cannot be a string";
        private const string RIGHT_STRING_ERROR = "The right operand of subtraction cannot be a string";

        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public override ExpressionValue Calculate(IEvaluationContext evalContext)
        {
            var left = LeftOperand.Evaluate(evalContext);
            var right = RightOperand.Evaluate(evalContext);
            switch (left.Type)
            {
                case ExpressionValueType.Bool:
                case ExpressionValueType.Integer:
                    var leftNum = left.AsLong();
                    switch (right.Type)
                    {
                        case ExpressionValueType.Bool:
                        case ExpressionValueType.Integer:
                            return new ExpressionValue(leftNum - right.AsLong());
                        case ExpressionValueType.Real:
                            return new ExpressionValue(leftNum - right.AsReal());
                        case ExpressionValueType.String:
                            EvaluationError = RIGHT_STRING_ERROR;
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.Real:
                    var leftReal = left.AsReal();
                    switch (right.Type)
                    {
                        case ExpressionValueType.Bool:
                        case ExpressionValueType.Integer:
                            return new ExpressionValue(leftReal + right.AsLong());
                        case ExpressionValueType.Real:
                            return new ExpressionValue(leftReal + right.AsReal());
                        case ExpressionValueType.String:
                            EvaluationError = RIGHT_STRING_ERROR;
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.String:
                    EvaluationError = LEFT_STRING_ERROR;
                    return ExpressionValue.Error;

                default:
                    return ExpressionValue.Error;
            }
        }
    }
}