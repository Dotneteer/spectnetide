namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents a single literal node
    /// </summary>
    public sealed class LiteralNode : ExpressionNode
    {
        /// <summary>
        /// The value of the literal node
        /// </summary>
        public uint LiteralValue { get; }

        /// <summary>
        /// The source of the literal node
        /// </summary>
        public string Source { get; }

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
        /// <param name="source">Source representation</param>
        public LiteralNode(uint value, string source)
        {
            LiteralValue = new ExpressionValue(value);
            Source = source;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => Source;
    }
}