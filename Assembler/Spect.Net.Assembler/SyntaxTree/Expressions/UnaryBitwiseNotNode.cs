using System;
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
                    return new ExpressionValue(~(long)oper.AsReal());
                case ExpressionValueType.String:
                    if (long.TryParse(oper.AsString(), out var longVar))
                    {
                        return new ExpressionValue(~longVar);
                    }
                    if (double.TryParse(oper.AsString(), out var realVar))
                    {
                        return new ExpressionValue(~(long)realVar);
                    }
                    throw new InvalidOperationException("Cannot convert string to a number");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}