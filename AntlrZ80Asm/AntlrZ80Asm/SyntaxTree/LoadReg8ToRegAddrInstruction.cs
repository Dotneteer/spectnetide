using AntlrZ80Asm.SyntaxTree.Expressions;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an LD ('16BitReg'), '8BitReg' instruction
    /// </summary>
    /// <remarks>
    /// Examples: 
    ///     ld (bc),a
    ///     ld (de),a
    /// </remarks>
    public class LoadReg8ToRegAddrInstruction : OperationBase
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