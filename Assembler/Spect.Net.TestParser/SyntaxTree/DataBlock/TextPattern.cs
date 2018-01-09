namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a text pattern
    /// </summary>
    public class TextPattern : MemoryPattern
    {
        /// <summary>
        /// The span of the string
        /// </summary>
        public TextSpan StringSpan { get; set; }

        /// <summary>
        /// The string itself
        /// </summary>
        public string String { get; set; }
    }
}