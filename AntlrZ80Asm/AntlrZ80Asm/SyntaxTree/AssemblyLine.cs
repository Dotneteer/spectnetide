namespace AntlrZ80Asm.SyntaxTree
{
    /// <summary>
    /// This class represents a single line of Z80 assembly in the syntax tree
    /// </summary>
    public abstract class AssemblyLine
    {
        /// <summary>
        /// The optional label
        /// </summary>
        public string Label { get; set; }
    }
}