namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents a load instruction
    /// </summary>
    public class LoadReg8ToReg8Instruction : Instruction
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