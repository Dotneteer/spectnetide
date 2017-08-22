namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the bitwise XOR operation
    /// </summary>
    public sealed class BitwiseXorOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Calculates the bitwise XOR of the two operands
        /// </summary>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate()
            => (ushort)(LeftOperand.Evaluate() ^ RightOperand.Evaluate());
    }
}