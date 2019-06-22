using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an UNARY + operation
    /// </summary>
    public sealed class UnaryPlusNode: UnaryExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext) 
            => Operand.Evaluate(evalContext);

        public UnaryPlusNode(Z80AsmParser.UnaryPlusExprContext context, Z80AsmVisitor visitor) 
            : base(context.expr(), visitor)
        {
        }
    }
}