namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents the current assembly address
    /// </summary>
    public sealed class CurrentAddresNode : ExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the symbol
        /// </summary>
        /// <returns>Symbol value</returns>
        public override ushort Evaluate()
        {
            // TODO: return the current assembly address
            return 0;
        }
    }
}