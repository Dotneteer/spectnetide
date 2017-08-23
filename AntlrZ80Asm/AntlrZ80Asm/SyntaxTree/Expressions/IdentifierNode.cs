namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This node represents an identifier as an expression
    /// </summary>
    public sealed class IdentifierNode : ExpressionNode
    {
        /// <summary>
        /// The name of the symbol
        /// </summary>
        public string SymbolName { get; set; }

        /// <summary>
        /// Retrieves the value of the symbol
        /// </summary>
        /// <returns>Symbol value</returns>
        public override ushort Evaluate()
        {
            // TODO: look up the symbol in the symblo table
            return 0;
        }
    }
}