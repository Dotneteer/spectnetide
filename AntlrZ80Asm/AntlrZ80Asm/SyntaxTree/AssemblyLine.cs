namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents a single line of Z80 assembly in the syntax tree
    /// </summary>
    public abstract class AssemblyLine
    {
        /// <summary>
        /// The optional label
        /// </summary>
        public string Label { get; set; }
    }

    /// <summary>
    /// This class represents an instruction
    /// </summary>
    public abstract class InstructionLine : AssemblyLine
    {
        /// <summary>
        /// Menmonic of the instruction (in UPPERCASE)
        /// </summary>
        public string Mnemonic { get; set; }
    }

    /// <summary>
    /// This class represents a trivial instruction that contains a single mnemonic
    /// without any additional parameter
    /// </summary>
    public sealed class TrivialInstruction : InstructionLine
    {
    }

    /// <summary>
    /// This class represents a pragma
    /// </summary>
    public abstract class PragmaLine : AssemblyLine
    {
    }


    /// <summary>
    /// A concrete assembly line
    /// </summary>
    public class ConcreteLine : AssemblyLine
    {
    }
}