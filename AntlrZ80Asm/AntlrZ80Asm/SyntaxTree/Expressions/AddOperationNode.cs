namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the add operation
    /// </summary>
    public sealed class AddOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Adds the two operands
        /// </summary>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate()
            => (ushort)(LeftOperand.Evaluate() + RightOperand.Evaluate());
    }
}