using System;
using System.Collections.Generic;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents the output of the compiler
    /// </summary>
    public class AssemblerOutput
    {
        private DateTime _startTime;

        /// <summary>
        /// Sets the total time of compilation
        /// </summary>
        public TimeSpan CompilationTime { get; private set; }

        /// <summary>
        /// The segments of the compilation output
        /// </summary>
        public List<BinarySegment> Segments { get; } = new List<BinarySegment>();

        /// <summary>
        /// The symbol table with properly defined symbols
        /// </summary>
        public Dictionary<string, ushort> Symbols { get; } =
            new Dictionary<string, ushort>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The list of fixups to carry out as the last phase of the compilation
        /// </summary>
        public List<FixupEntry> Fixups { get; } = new List<FixupEntry>();

        /// <summary>
        /// The errors found during the compilation
        /// </summary>
        public List<AssemblerErrorInfo> Errors { get; } = new List<AssemblerErrorInfo>();

        /// <summary>
        /// Number of compilation errors
        /// </summary>
        public int ErrorCount => Errors.Count;

        /// <summary>
        /// Entry address of the code
        /// </summary>
        public ushort? EntryAddress { get; set; }

        /// <summary>
        /// Signs that the compilation started
        /// </summary>
        public void StartCompilation()
        {
            _startTime = DateTime.Now;
        }

        /// <summary>
        /// Signs that the compilation has been finished
        /// </summary>
        public void CompleteCompilation()
        {
            CompilationTime = DateTime.Now - _startTime;
        }
    }
}