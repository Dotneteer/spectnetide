namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the multiplication operation
    /// </summary>
    public sealed class MultiplyOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Multiplies the two operands
        /// </summary>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate()
            => (ushort)(LeftOperand.Evaluate() * RightOperand.Evaluate());
    }
}