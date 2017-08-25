namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an increment instruction
    /// </summary>
    public class IncrementInstruction : Instruction
    {
        /// <summary>
        /// Target of increment
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Optional indexed address
        /// </summary>
        public IndexedAddress IndexedAddress { get; set; }
    }
}