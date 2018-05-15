namespace Spect.Net.Assembler.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents a single literal node
    /// </summary>
    public sealed class LiteralNode : ExpressionNode
    {
        public ExpressionValue LiteralValue { get; }

        /// <summary>
        /// The value of the literal node as a Boolean
        /// </summary>
        public bool AsBool => LiteralValue.AsBool();

        /// <summary>
        /// The value of the literal node as a 16-bit integer
        /// </summary>
        public ushort AsWord => LiteralValue.AsWord();

        /// <summary>
        /// The value of the literal node as real number
        /// </summary>
        public double AsReal => LiteralValue.AsReal();

        /// <summary>
        /// The value of the literal node as a string
        /// </summary>
        public string AsString => LiteralValue.AsString();

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext) => LiteralValue;

        /// <summary>
        /// Initialize an ushort literal value
        /// </summary>
        /// <param name="value">Ushort value</param>
        public LiteralNode(ExpressionValue value)
        {
            LiteralValue = value;
        }

        /// <summary>
        /// Initialize a Boolean literal value
        /// </summary>
        /// <param name="value">Boolean value</param>
        public LiteralNode(bool value)
        {
            LiteralValue = new ExpressionValue(value);
        }

        /// <summary>
        /// Initialize an ushort literal value
        /// </summary>
        /// <param name="value">Ushort value</param>
        public LiteralNode(ushort value)
        {
            LiteralValue = new ExpressionValue(value);
        }

        /// <summary>
        /// Initialize a long literal value
        /// </summary>
        /// <param name="value">long value</param>
        public LiteralNode(long value)
        {
            LiteralValue = new ExpressionValue(value);
        }

        /// <summary>
        /// Initialize a double literal value
        /// </summary>
        /// <param name="value">Double value</param>
        public LiteralNode(double value)
        {
            LiteralValue = new ExpressionValue(value);
        }

        /// <summary>
        /// Initialize a string literal value
        /// </summary>
        /// <param name="value">String value</param>
        public LiteralNode(string value)
        {
            LiteralValue = new ExpressionValue(value);
        }
    }
}