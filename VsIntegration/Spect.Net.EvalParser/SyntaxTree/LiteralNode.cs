namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents a single literal node
    /// </summary>
    public sealed class LiteralNode : ExpressionNode
    {
        public ExpressionValue LiteralValue { get; }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext) => LiteralValue;

        /// <summary>
        /// Initialize an ushort literal value
        /// </summary>
        /// <param name="value">Ushort value</param>
        public LiteralNode(ExpressionValue value)
        {
            LiteralValue = value;
        }

        /// <summary>
        /// Initialize a double literal value
        /// </summary>
        /// <param name="value">Double value</param>
        public LiteralNode(uint value)
        {
            LiteralValue = new ExpressionValue(value);
        }
    }
}