using System;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the modulo operation
    /// </summary>
    public sealed class ModuloOperationNode : BinaryOperationNode
    {
        private const string LEFT_STRING_ERROR = "The left operand of modulo operation cannot be a string";
        private const string RIGHT_STRING_ERROR = "The right operand of modulo operation cannot be a string";
        private const string DIV_BY_ZERO_ERROR = "Divide by zero error";

        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public override ExpressionValue Calculate(IEvaluationContext evalContext)
        {
            var left = LeftOperand.Evaluate(evalContext);
            var right = RightOperand.Evaluate(evalContext);
            switch (right.Type)
            {
                case ExpressionValueType.Bool:
                case ExpressionValueType.Integer:
                    var rightNum = right.AsLong();
                    if (rightNum == 0)
                    {
                        EvaluationError = DIV_BY_ZERO_ERROR;
                        return ExpressionValue.Error;
                    }
                    switch (left.Type)
                    {
                        case ExpressionValueType.Bool:
                        case ExpressionValueType.Integer:
                            return new ExpressionValue(left.AsLong() % rightNum);
                        case ExpressionValueType.Real:
                            return new ExpressionValue(left.AsReal() % rightNum);
                        case ExpressionValueType.String:
                            EvaluationError = LEFT_STRING_ERROR;
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.Real:
                    var rightReal = right.AsReal();
                    if (Math.Abs(rightReal) < double.Epsilon)
                    {
                        EvaluationError = DIV_BY_ZERO_ERROR;
                        return ExpressionValue.Error;
                    }
                    switch (left.Type)
                    {
                        case ExpressionValueType.Bool:
                        case ExpressionValueType.Integer:
                            return new ExpressionValue(left.AsLong() % rightReal);
                        case ExpressionValueType.Real:
                            return new ExpressionValue(left.AsReal() % rightReal);
                        case ExpressionValueType.String:
                            EvaluationError = LEFT_STRING_ERROR;
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.String:
                    EvaluationError = RIGHT_STRING_ERROR;
                    return ExpressionValue.Error;

                default:
                    return ExpressionValue.Error;
            }
        }
    }
}