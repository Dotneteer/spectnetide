namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an instruction
    /// </summary>
    public abstract class OperationBase : AssemblyLine
    {
        /// <summary>
        /// Menmonic of the instruction (in UPPERCASE)
        /// </summary>
        public string Mnemonic { get; set; }
    }
}