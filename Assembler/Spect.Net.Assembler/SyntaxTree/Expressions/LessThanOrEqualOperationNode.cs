using System;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the 'less than or equal' operation
    /// </summary>
    public sealed class LessThanOrEqualOperationNode : BinaryOperationNode
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
                            return new ExpressionValue(leftNum <= right.AsLong());
                        case ExpressionValueType.Real:
                            return new ExpressionValue(leftNum <= right.AsReal());
                        case ExpressionValueType.String:
                            EvaluationError = "Cannot compare an integer number with a string";
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
                            return new ExpressionValue(leftReal <= right.AsLong());
                        case ExpressionValueType.Real:
                            return new ExpressionValue(leftReal <= right.AsReal());
                        case ExpressionValueType.String:
                            EvaluationError = "Cannot compare an integer number with a string";
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.String:
                    if (right.Type == ExpressionValueType.String)
                    {
                        return new ExpressionValue(string.CompareOrdinal(left.AsString(), right.AsString()) <= 0);
                    }

                    EvaluationError = "String can be compared only to another string";
                    return ExpressionValue.Error;

                default:
                    return ExpressionValue.Error;
            }
        }

        public LessThanOrEqualOperationNode(object leftOperand, object rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }
}