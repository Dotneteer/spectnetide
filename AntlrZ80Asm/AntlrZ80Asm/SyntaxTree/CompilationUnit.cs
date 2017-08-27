using System.Collections.Generic;

namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents the entire syntax tree of a Z80 Assembly program
    /// </summary>
    public class CompilationUnit
    {
        /// <summary>
        /// The lines of the program
        /// </summary>
        public List<SourceLineBase> Lines { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public CompilationUnit()
        {
            Lines = new List<SourceLineBase>();
        }
    }
}