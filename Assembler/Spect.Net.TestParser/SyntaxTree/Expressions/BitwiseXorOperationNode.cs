using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
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
                    EvaluationError = "Cannot apply bitwise XOR on a numeric value and a byte array";
                    return ExpressionValue.Error;
                }

                // --- Bitwise XOR each array elements (shortest)
                var leftArray = leftValue.AsByteArray();
                var rightArray = rightValue.AsByteArray();
                var shortest = leftArray.Length > rightArray.Length ? leftArray.Length : rightArray.Length;
                var resultArray = new byte[shortest];
                for (var i = 0; i < shortest; i++)
                {
                    resultArray[i] = (byte)(leftArray[i] ^ rightArray[i]);
                }
                return new ExpressionValue(resultArray);
            }

            // --- Test incompatible types
            if (rightValue.Type == ExpressionValueType.ByteArray)
            {
                EvaluationError = "Cannot apply bitwise XOR on a byte array and a numeric value";
                return ExpressionValue.Error;
            }

            // --- Numeric operands
            return new ExpressionValue((ushort)(leftValue.AsNumber() ^ rightValue.AsNumber()));
        }

        public BitwiseXorOperationNode(ParserRuleContext context) : base(context)
        {
        }
    }
}