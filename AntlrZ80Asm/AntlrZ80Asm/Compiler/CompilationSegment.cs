using System.Collections.Generic;

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
        public int Displacement { get; set; }
        
        /// <summary>
        /// Emitted Z80 binary code
        /// </summary>
        public List<byte> EmittedCode { get; set; } = new List<byte>(1024);

        /// <summary>
        /// The current code generation offset
        /// </summary>
        public int CurrentOffset => EmittedCode.Count;
    }
}