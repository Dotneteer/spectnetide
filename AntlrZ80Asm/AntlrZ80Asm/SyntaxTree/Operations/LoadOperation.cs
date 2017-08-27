namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a load operation
    /// </summary>
    public class LoadOperation : OperationBase
    {
        /// <summary>
        /// Source of the LD operation
        /// </summary>
        public Operand SourceOperand { get; set; }

        /// <summary>
        /// Destination of the LD operation
        /// </summary>
        public Operand DestinationOperand { get; set; }
    }
}