namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents a single literal node
    /// </summary>
    public sealed class LiteralNode : ExpressionNode
    {
        /// <summary>
        /// The value of the literal node
        /// </summary>
        public ushort LiteralValue { get; set; }

        /// <summary>
        /// Retrieves the value of the literal
        /// </summary>
        public override ushort Evaluate() => LiteralValue;
    }
}