namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the bitwise OR operation
    /// </summary>
    public sealed class BitwiseOrOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Calculates the bitwise OR of the two operands
        /// </summary>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate()
            => (ushort)(LeftOperand.Evaluate() | RightOperand.Evaluate());
    }
}