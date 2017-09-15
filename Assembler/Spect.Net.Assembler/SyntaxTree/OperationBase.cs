namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents an instruction
    /// </summary>
    public abstract class OperationBase : SourceLineBase
    {
        /// <summary>
        /// Menmonic of the instruction (in UPPERCASE)
        /// </summary>
        public string Mnemonic { get; set; }
    }
}