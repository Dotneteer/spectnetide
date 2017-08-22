namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents a load instruction
    /// </summary>
    public class LoadInstruction : InstructionLine
    {
        /// <summary>
        /// The type of load instruction
        /// </summary>
        public LoadType LoadType { get; set; }

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