namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the bitwise AND operation
    /// </summary>
    public sealed class BitwiseAndOperationNode : BinaryOperationNode
    {
        private const string RIGHT_OPER_ERROR = "The right operand of bitwise AND operation can only be an integral type";
        private const string LEFT_OPER_ERROR = "The left operand of bitwise AND operation can only be an integral type";

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
                            return new ExpressionValue(leftNum & right.AsLong());
                        case ExpressionValueType.Real:
                        case ExpressionValueType.String:
                            EvaluationError = RIGHT_OPER_ERROR;
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.String:
                    if (right.Type == ExpressionValueType.String)
                    {
                        return new ExpressionValue($"{left.AsString()}\r\n{right.AsString()}");
                    }
                    EvaluationError = LEFT_OPER_ERROR;
                    return ExpressionValue.Error;

                case ExpressionValueType.Real:
                    EvaluationError = LEFT_OPER_ERROR;
                    return ExpressionValue.Error;

                default:
                    return ExpressionValue.Error;
            }
        }
    }
}