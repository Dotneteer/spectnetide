using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the 'greater than' operation
    /// </summary>
    public sealed class GreaterThanOperationNode : BinaryOperationNode
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
                            return new ExpressionValue(leftNum > right.AsLong());
                        case ExpressionValueType.Real:
                            return new ExpressionValue(leftNum > right.AsReal());
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
                            return new ExpressionValue(leftReal > right.AsLong());
                        case ExpressionValueType.Real:
                            return new ExpressionValue(leftReal > right.AsReal());
                        case ExpressionValueType.String:
                            EvaluationError = "Cannot compare an integer number with a string";
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.String:
                    if (right.Type == ExpressionValueType.String)
                    {
                        return new ExpressionValue(string.CompareOrdinal(left.AsString(), right.AsString()) > 0);
                    }

                    EvaluationError = "String can be compared only to another string";
                    return ExpressionValue.Error;

                default:
                    return ExpressionValue.Error;
            }
        }

        public GreaterThanOperationNode(Z80AsmParser.RelExprContext context, Z80AsmVisitor visitor)
            : base(context, context.expr()[0], context.expr()[1], visitor)
        {
        }
    }
}