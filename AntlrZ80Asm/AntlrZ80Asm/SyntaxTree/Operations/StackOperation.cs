namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a stack operation
    /// </summary>
    public sealed class StackOperation : OperationBase
    {
        /// <summary>
        /// Associated register
        /// </summary>
        public string Register { get; set; }
    }
}