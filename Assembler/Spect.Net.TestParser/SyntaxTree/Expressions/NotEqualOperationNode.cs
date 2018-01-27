using System.Linq;
using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the 'not equal' operation
    /// </summary>
    public sealed class NotEqualOperationNode : BinaryOperationNode
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

            // --- Test if both operands are byte arrays
            if (leftValue.Type == ExpressionValueType.ByteArray)
            {
                if (rightValue.Type != ExpressionValueType.ByteArray)
                {
                    EvaluationError = "Byte array can be compared only to byte array";
                    return ExpressionValue.Error;
                }
                return new ExpressionValue(!leftValue.AsByteArray().SequenceEqual(rightValue.AsByteArray()));
            }

            // --- Test for incompatible types
            if (rightValue.Type == ExpressionValueType.ByteArray)
            {
                EvaluationError = "Cannot compare a byte array with a numeric value";
                return ExpressionValue.Error;
            }

            // --- Numeric operands
            return new ExpressionValue((ushort)(leftValue.AsNumber() >> (ushort)rightValue.AsNumber()));
        }

        public NotEqualOperationNode(ParserRuleContext context) : base(context)
        {
        }
    }
}