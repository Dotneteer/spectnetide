using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an LD '8BitReg', ('16BitReg') instruction
    /// </summary>
    /// <remarks>
    /// Examples: 
    ///     ld a,(bc)
    ///     ld a,(de)
    /// </remarks>
    public class LoadReg8FromRegAddrInstruction : Instruction
    {
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