using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the shift left operation
    /// </summary>
    public sealed class ShiftLeftOperationNode : BinaryOperationNode
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
            if (left.Type != ExpressionValueType.Bool && left.Type != ExpressionValueType.Integer)
            {
                EvaluationError = "The left operand of the shift left operator must be integral";
                return ExpressionValue.Error;
            }
            if (right.Type != ExpressionValueType.Bool && right.Type != ExpressionValueType.Integer)
            {
                EvaluationError = "Right operand of the shift left operator must be integral";
                return ExpressionValue.Error;
            }
            return new ExpressionValue(left.AsLong() << (ushort)right.AsLong());
        }

        public ShiftLeftOperationNode(Z80AsmParser.ShiftExprContext context, Z80AsmVisitor visitor)
            : base(context, context.expr()[0], context.expr()[1], visitor)
        {
        }
    }
}