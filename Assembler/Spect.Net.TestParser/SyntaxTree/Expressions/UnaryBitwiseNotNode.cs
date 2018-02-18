using System;
using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an UNARY bitwise NOT operation
    /// </summary>
    public sealed class UnaryBitwiseNotNode : UnaryExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <param name="checkOnly"></param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext, bool checkOnly = false)
        {
            // --- Check operand error
            var operandValue = Operand.Evaluate(evalContext);
            if (operandValue.Type == ExpressionValueType.Error)
            {
                EvaluationError = Operand.EvaluationError;
                return ExpressionValue.Error;
            }

            if (checkOnly) return ExpressionValue.NonEvaluated;

            // --- Carry out operation
            switch (operandValue.Type)
            {
                case ExpressionValueType.ByteArray:
                    var opArray = operandValue.AsByteArray();
                    var result = new byte[opArray.Length];
                    for (var i = 0; i < result.Length; i++)
                    {
                        result[i] = (byte)~opArray[i];
                    }
                    return new ExpressionValue(result);
                case ExpressionValueType.Bool:
                case ExpressionValueType.Number:
                    return new ExpressionValue((ushort)~operandValue.AsNumber());
                default:
                    return ExpressionValue.Error;
            }
        }

        public UnaryBitwiseNotNode(ParserRuleContext context) : base(context)
        {
        }
    }
}