namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents a single literal node
    /// </summary>
    public sealed class LiteralNode : ExpressionNode
    {
        public uint LiteralValue { get; }

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            if (LiteralValue <= byte.MaxValue)
            {
                SuggestType(ExpressionValueType.Byte);
            }
            else if (LiteralValue <= ushort.MaxValue)
            {
                SuggestType(ExpressionValueType.Word);
            }
            else
            {
                SuggestType(ExpressionValueType.DWord);
            }
            return new ExpressionValue(LiteralValue);
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