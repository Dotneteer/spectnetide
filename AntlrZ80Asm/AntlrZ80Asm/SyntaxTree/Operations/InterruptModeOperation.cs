namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents an interrupt mode operation
    /// </summary>
    public sealed class InterruptModeOperation : OperationBase
    {
        /// <summary>
        /// Associated register
        /// </summary>
        public string Mode { get; set; }
    }
}