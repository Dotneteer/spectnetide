namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an EX 'dest', 'source' instruction
    /// </summary>
    /// <remarks>
    /// Examples: 
    ///     ex de, hl
    ///     ex (sp), ix
    /// </remarks>
    public class ExchangeInstruction : Instruction
    {
        /// <summary>
        /// LD instruction destination
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// LD instruction source destination
        /// </summary>
        public string Source { get; set; }
    }
}