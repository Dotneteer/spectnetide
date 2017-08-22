namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the shift rightí operation
    /// </summary>
    public sealed class ShiftRightOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Calculates the shift right value of the two operands
        /// </summary>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate()
            => (ushort)(LeftOperand.Evaluate() >> RightOperand.Evaluate());
    }
}