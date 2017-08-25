namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an decrement instruction
    /// </summary>
    public class DecrementInstruction : Instruction
    {
        /// <summary>
        /// Target of decrement
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Optional indexed address
        /// </summary>
        public IndexedAddress IndexedAddress { get; set; }
    }
}