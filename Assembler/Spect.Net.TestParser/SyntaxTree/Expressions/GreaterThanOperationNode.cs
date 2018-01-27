using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
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
        public override ExpressionValue Calculate(IExpressionEvaluationContext evalContext)
        {
            // --- Check operands for errors
            var leftValue = LeftOperand.Evaluate(evalContext);
            if (leftValue.Type == ExpressionValueType.Error)
            {
                EvaluationError = LeftOperand.EvaluationError;
                return ExpressionValue.Error;
            }
            var rightValue = RightOperand.Evaluate(evalContext);
            if (rightValue.Type == ExpressionValueType.Error)
            {
                EvaluationError = RightOperand.EvaluationError;
                return ExpressionValue.Error;
            }

            // --- Test for incompatible types
            if (leftValue.Type == ExpressionValueType.ByteArray || rightValue.Type == ExpressionValueType.ByteArray)
            {
                EvaluationError = "'>' operator cannot be applied if any of the operands is a byte array";
                return ExpressionValue.Error;
            }

            return new ExpressionValue(leftValue.AsNumber() > rightValue.AsNumber());
        }

        public GreaterThanOperationNode(ParserRuleContext context) : base(context)
        {
        }
    }
}