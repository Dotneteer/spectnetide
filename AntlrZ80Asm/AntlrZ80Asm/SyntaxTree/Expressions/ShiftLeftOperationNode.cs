namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the shift left operation
    /// </summary>
    public sealed class ShiftLeftOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Calculates the shift left value of the two operands
        /// </summary>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate()
            => (ushort)(LeftOperand.Evaluate() << RightOperand.Evaluate());
    }
}