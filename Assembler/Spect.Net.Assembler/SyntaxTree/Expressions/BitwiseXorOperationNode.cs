namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the bitwise XOR operation
    /// </summary>
    public sealed class BitwiseXorOperationNode : BinaryOperationNode
    {
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
                            return new ExpressionValue(leftNum ^ right.AsLong());
                        case ExpressionValueType.Real:
                            return new ExpressionValue(leftNum ^ (long)right.AsReal());
                        case ExpressionValueType.String:
                            EvaluationError = "Cannot use bitwise XOR between an integer number and a string";
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
                            return new ExpressionValue((long)leftReal ^ right.AsLong());
                        case ExpressionValueType.Real:
                            return new ExpressionValue((long)leftReal ^ (long)right.AsReal());
                        case ExpressionValueType.String:
                            EvaluationError = "Cannot use bitwise XOR between a real number and a string";
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.String:
                    EvaluationError = "The left operand of bitwise XOR cannot be a string";
                    return ExpressionValue.Error;

                default:
                    return ExpressionValue.Error;
            }
        }
    }
}