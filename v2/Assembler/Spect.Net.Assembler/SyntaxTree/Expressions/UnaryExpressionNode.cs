using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an abstract unary operation
    /// </summary>
    public abstract class UnaryExpressionNode : ExpressionNode
    {
        /// <summary>
        /// Operand of the unary operation
        /// </summary>
        public ExpressionNode Operand { get; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IEvaluationContext evalContext)
            => Operand.ReadyToEvaluate(evalContext);

        protected UnaryExpressionNode(Z80AsmParser.ExprContext context, Z80AsmVisitor visitor)
            : base(context)
        {
            Operand = (ExpressionNode)visitor.VisitExpr(context);
        }
    }
}