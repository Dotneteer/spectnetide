using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the bitwise XOR operation
    /// </summary>
    public sealed class BitwiseXorOperationNode : BinaryOperationNode
    {
        private const string RIGHT_OPER_ERROR = "The right operand of bitwise XOR operation can only be an integral type";
        private const string LEFT_OPER_ERROR = "The left operand of bitwise XOR operation can only be an integral type";

        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public override ExpressionValue Calculate(IEvaluationContext evalContext)
        {
            var left = LeftOperand.Evaluate(evalContext);
            var right = RightOperand.Evaluate(evalContext);
            switch (left.Type)
            {
                case ExpressionValueType.Bool:
                case ExpressionValueType.Integer:
                    var leftNum = left.AsLong();
                    switch (right.Type)
                    {
                        case ExpressionValueType.Bool:
                        case ExpressionValueType.Integer:
                            return new ExpressionValue(leftNum ^ right.AsLong());
                        case ExpressionValueType.Real:
                        case ExpressionValueType.String:
                            EvaluationError = RIGHT_OPER_ERROR;
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.Real:
                case ExpressionValueType.String:
                    EvaluationError = LEFT_OPER_ERROR;
                    return ExpressionValue.Error;

                default:
                    return ExpressionValue.Error;
            }
        }

        public BitwiseXorOperationNode(Z80AsmParser.XorExprContext context, Z80AsmVisitor visitor)
            : base(context, context.expr()[0], context.expr()[1], visitor)
        {
        }
    }
}