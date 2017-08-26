namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents an LD '8BitReg', '8BitReg' instruction
    /// </summary>
    /// <remarks>
    /// Examples: 
    ///     ld b,a
    ///     ld (hl),c
    /// </remarks>
    public class LoadReg8ToReg8Instruction : OperationBase
    {
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