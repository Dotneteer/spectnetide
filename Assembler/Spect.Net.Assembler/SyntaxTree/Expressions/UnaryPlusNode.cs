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
        public override ushort Evaluate(IEvaluationContext evalContext) 
            => Operand.Evaluate(evalContext);
    }
}