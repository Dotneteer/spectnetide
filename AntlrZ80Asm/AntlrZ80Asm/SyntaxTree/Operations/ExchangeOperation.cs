namespace AntlrZ80Asm.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents and exchange operation
    /// </summary>
    public class ExchangeOperation : OperationBase
    {
        /// <summary>
        /// EX destination
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// EX source
        /// </summary>
        public string Source { get; set; }
    }
}