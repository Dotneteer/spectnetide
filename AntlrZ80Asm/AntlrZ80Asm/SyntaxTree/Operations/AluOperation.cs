namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// Represents an ALU operation
    /// </summary>
    public sealed class AluOperation : OperationBase
    {
        /// <summary>
        /// The name of the destination register, if not A
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// Increment/Decrement operand
        /// </summary>
        public Operand Operand { get; set; }
    }
}