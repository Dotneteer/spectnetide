using System.Collections.Generic;

namespace AntlrZ80Asm.Compiler
{
    /// <summary>
    /// This class represents the output of the compiler
    /// </summary>
    public class CompilerOutput
    {
        /// <summary>
        /// The segments of the compilation output
        /// </summary>
        public List<CompilationSegment> Segments { get; } = new List<CompilationSegment>();

        /// <summary>
        /// The errors found during the compilation
        /// </summary>
        public List<CompilerErrorInfoBase> Errors { get; } = new List<CompilerErrorInfoBase>();

        /// <summary>
        /// Number of compilation errors
        /// </summary>
        public int ErrorCount => Errors.Count;
    }
}