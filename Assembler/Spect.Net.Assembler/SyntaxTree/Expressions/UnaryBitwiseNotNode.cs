using System;
using Spect.Net.Assembler.Generated;

// ReSharper disable SwitchStatementMissingSomeCases

namespace Spect.Net.Assembler.SyntaxTree.Expressions
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
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            var oper = Operand.Evaluate(evalContext);
            switch (oper.Type)
            {
                case ExpressionValueType.Bool:
                case ExpressionValueType.Integer:
                    return new ExpressionValue(~oper.AsLong());
                case ExpressionValueType.Real:
                case ExpressionValueType.String:
                    EvaluationError = "Unary bitwise not operation can be applied only on integral types";
                    return ExpressionValue.Error;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public UnaryBitwiseNotNode(Z80AsmParser.BinaryNotExprContext context, Z80AsmVisitor visitor)
            : base(context.expr(), visitor)
        {
        }
    }
}