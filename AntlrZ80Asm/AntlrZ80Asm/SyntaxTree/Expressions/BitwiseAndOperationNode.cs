namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the bitwise AND operation
    /// </summary>
    public sealed class BitwiseAndOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Calculates the bitwise AND of the two operands
        /// </summary>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate()
            => (ushort)(LeftOperand.Evaluate() & RightOperand.Evaluate());
    }
}