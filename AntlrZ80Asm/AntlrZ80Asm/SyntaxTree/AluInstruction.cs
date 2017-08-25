namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents ALU instructions
    /// </summary>
    public class AluInstruction : Instruction
    {
        /// <summary>
        /// Instruction type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// LD instruction destination
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// LD instruction source destination
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Optional indexed address
        /// </summary>
        public IndexedAddress IndexedAddress { get; set; }
    }
}