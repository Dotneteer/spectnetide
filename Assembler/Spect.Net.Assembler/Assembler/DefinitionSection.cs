namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// Defines a section of assembly lines
    /// </summary>
    public struct DefinitionSection
    {
        /// <summary>
        /// First line index (inclusive)
        /// </summary>
        public int FirstLine { get; }

        /// <summary>
        /// Last line index (inclusive)
        /// </summary>
        public int LastLine { get; }

        /// <summary>
        /// Initializes a new instance of the structure
        /// </summary>
        public DefinitionSection(int firstLine, int lastLine)
        {
            FirstLine = firstLine;
            LastLine = lastLine;
        }
    }
}