using System.Collections.Generic;

namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class represents the output of the compiler
    /// </summary>
    public class AssemblerOutput
    {
        /// <summary>
        /// The segments of the compilation output
        /// </summary>
        public List<BinarySegment> Segments { get; } = new List<BinarySegment>();

        /// <summary>
        /// The errors found during the compilation
        /// </summary>
        public List<AssemblerErrorInfoBase> Errors { get; } = new List<AssemblerErrorInfoBase>();

        /// <summary>
        /// Number of compilation errors
        /// </summary>
        public int ErrorCount => Errors.Count;

        /// <summary>
        /// Entry address of the code
        /// </summary>
        public ushort? EntryAddress { get; set; }
    }
}