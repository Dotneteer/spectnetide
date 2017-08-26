namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// Represents an increment or decrement operation
    /// </summary>
    public sealed class IncDecOperation : OperationBase
    {
        /// <summary>
        /// Increment/Decrement operand
        /// </summary>
        public Operand Operand { get; set; }
    }
}