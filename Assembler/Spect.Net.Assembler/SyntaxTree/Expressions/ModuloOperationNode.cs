using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the modulo operation
    /// </summary>
    public sealed class ModuloOperationNode : BinaryOperationNode
    {
        private const string LEFT_OPER_ERROR = "The left operand of modulo operation can only be an integral type";
        private const string RIGHT_OPER_ERROR = "The right operand of modulo operation can only be an integral type";
        private const string DIV_BY_ZERO_ERROR = "Divide by zero error";

        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public override ExpressionValue Calculate(IEvaluationContext evalContext)
        {
            var left = LeftOperand.Evaluate(evalContext);
            var right = RightOperand.Evaluate(evalContext);
            switch (right.Type)
            {
                case ExpressionValueType.Bool:
                case ExpressionValueType.Integer:
                    var rightNum = right.AsLong();
                    if (rightNum == 0)
                    {
                        EvaluationError = DIV_BY_ZERO_ERROR;
                        return ExpressionValue.Error;
                    }
                    switch (left.Type)
                    {
                        case ExpressionValueType.Bool:
                        case ExpressionValueType.Integer:
                            return new ExpressionValue(left.AsLong() % rightNum);
                        case ExpressionValueType.Real:
                        case ExpressionValueType.String:
                            EvaluationError = LEFT_OPER_ERROR;
                            return ExpressionValue.Error;
                        default:
                            return ExpressionValue.Error;
                    }

                case ExpressionValueType.Real:
                case ExpressionValueType.String:
                    EvaluationError = RIGHT_OPER_ERROR;
                    return ExpressionValue.Error;

                default:
                    return ExpressionValue.Error;
            }
        }

        public ModuloOperationNode(Z80AsmParser.MultExprContext context, Z80AsmVisitor visitor)
            : base(context, context.expr()[0], context.expr()[1], visitor)
        {
        }
    }
}