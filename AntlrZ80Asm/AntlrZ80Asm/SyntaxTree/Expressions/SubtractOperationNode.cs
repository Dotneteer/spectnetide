namespace AntlrZ80Asm.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the subtract operation
    /// </summary>
    public sealed class SubtractOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Subtracts the right operand from the left one
        /// </summary>
        /// <returns>Result of the operation</returns>
        public override ushort Calculate()
            => (ushort)(LeftOperand.Evaluate() - RightOperand.Evaluate());
    }
}