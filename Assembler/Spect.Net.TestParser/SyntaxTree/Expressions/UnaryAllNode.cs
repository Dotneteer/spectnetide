using System.Linq;
using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an UNARY ALL operation
    /// </summary>
    public sealed class UnaryAllNode: UnaryExpressionNode
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
            return operandValue.Type != ExpressionValueType.ByteArray 
                ? new ExpressionValue(operandValue.AsBool()) 
                : new ExpressionValue(operandValue.AsByteArray().All(item => item != 0));
        }

        public UnaryAllNode(ParserRuleContext context) : base(context)
        {
        }
    }
}