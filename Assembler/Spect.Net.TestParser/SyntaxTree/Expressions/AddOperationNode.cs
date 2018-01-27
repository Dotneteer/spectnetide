using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
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
                    EvaluationError = "Cannot add a numeric value to a byte array";
                    return ExpressionValue.Error;
                }

                // --- Concatenate the two byte arrays
                var leftArray = leftValue.AsByteArray();
                var rightArray = rightValue.AsByteArray();
                var resultArray = new byte[leftArray.Length + rightArray.Length];
                System.Buffer.BlockCopy(leftArray, 0, resultArray, 0, leftArray.Length);
                System.Buffer.BlockCopy(rightArray, 0, resultArray, leftArray.Length, rightArray.Length);
                return new ExpressionValue(resultArray);
            }

            // --- Test for incompatible types
            if (rightValue.Type == ExpressionValueType.ByteArray)
            {
                EvaluationError = "Cannot add a byte array to a numeric value";
                return ExpressionValue.Error;
            }

            // --- Numeric operands
            return new ExpressionValue((ushort)(leftValue.AsWord() + rightValue.AsWord()));
        }

        public AddOperationNode(ParserRuleContext context) : base(context)
        {
        }
    }
}