namespace Spect.Net.Assembler.SyntaxTree
{
    /// <summary>
    /// This class represents a single line of Z80 assembly in the syntax tree
    /// </summary>
    public abstract class SourceLineBase
    {
        /// <summary>
        /// The source line number
        /// </summary>
        public int SourceLine { get; set; }

        /// <summary>
        /// The first position within a source code line
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The optional label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Label span information
        /// </summary>
        public TextSpan LabelSpan { get; set; }
    }
}