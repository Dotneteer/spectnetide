namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents a trivial instruction that contains a single mnemonic
    /// without any additional parameter
    /// </summary>
    public sealed class TrivialInstruction : Instruction
    {
        /// <summary>
        /// Menmonic of the instruction (in UPPERCASE)
        /// </summary>
        public string Mnemonic { get; set; }
    }
}