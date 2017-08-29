namespace AntlrZ80Asm.Compiler
{
    /// <summary>
    /// This class represents a single segment of the code compilation
    /// </summary>
    public class CompilationSegment
    {
        /// <summary>
        /// Start address of the compiled block
        /// </summary>
        public ushort StartAddress { get; set; }

        /// <summary>
        /// Displacement of the this segment
        /// </summary>
        public ushort Displacement { get; set; }
        
        /// <summary>
        /// Emitted Z80 binary code
        /// </summary>
        public byte[] EmittedCode { get; set; }
    }
}