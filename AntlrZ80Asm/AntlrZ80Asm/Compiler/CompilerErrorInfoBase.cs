namespace AntlrZ80Asm.Compiler
{
    /// <summary>
    /// This class represents a compilation error
    /// </summary>
    public abstract class CompilerErrorInfoBase
    {
        /// <summary>
        /// Source line of the error
        /// </summary>
        public int SourceLine { get; set; }

        /// <summary>
        /// Position within the source line
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Problematic source code
        /// </summary>
        public string ProblematicCode { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; protected set; }
    }
}