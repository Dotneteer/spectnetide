using System.Data;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents a single literal node
    /// </summary>
    public sealed class LiteralNode : ExpressionNode
    {
        /// <summary>
        /// The value of the literal node
        /// </summary>
        public ushort LiteralValueAsWord { get; set; }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext) => new ExpressionValue(LiteralValueAsWord);
    }
}