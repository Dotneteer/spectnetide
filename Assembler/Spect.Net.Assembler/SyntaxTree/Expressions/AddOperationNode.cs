namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the add operation
    /// </summary>
    public sealed class AddOperationNode : BinaryOperationNode
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
                            return new ExpressionValue(leftNum + right.AsLong());
                        case ExpressionValueType.Real:
                            return new ExpressionValue(leftNum + right.AsReal());
                        case ExpressionValueType.String:
                            return new ExpressionValue($"{leftNum}{right.AsString()}");
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
                            return new ExpressionValue($"{leftReal}{right.AsString()}");
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.String:
                    return new ExpressionValue($"{left.AsString()}{right.AsString()}");

                default:
                    return ExpressionValue.Error;
            }
        }
    }
}