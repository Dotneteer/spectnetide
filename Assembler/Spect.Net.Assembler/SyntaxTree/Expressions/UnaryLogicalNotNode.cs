using System;
using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an UNARY logical NOT operation
    /// </summary>
    public sealed class UnaryLogicalNotNode : UnaryExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            var oper = Operand.Evaluate(evalContext);
            switch (oper.Type)
            {
                case ExpressionValueType.Bool:
                case ExpressionValueType.Integer:
                    return new ExpressionValue(oper.AsLong() == 0);
                case ExpressionValueType.Real:
                case ExpressionValueType.String:
                    EvaluationError = "Unary logical not operation can be applied only on integral types";
                    return ExpressionValue.Error;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public UnaryLogicalNotNode(Z80AsmParser.LogicalNotExprContext context, Z80AsmVisitor visitor)
            : base(context.expr(), visitor)
        {
        }
    }
}