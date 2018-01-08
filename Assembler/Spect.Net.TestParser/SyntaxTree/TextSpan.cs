namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// This structure represents a text span with an inclusive start and
    /// an exclusive end position.
    /// </summary>
    public struct TextSpan
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TextSpan(int startLine, int startPosition, int endLine, int endPosition)
        {
            StartLine = startLine;
            StartPosition = startPosition;
            EndLine = endLine;
            EndPosition = endPosition;
        }

        /// <summary>
        /// The source line number
        /// </summary>
        public int StartLine { get; }

        /// <summary>
        /// The first position within a source code line
        /// </summary>
        public int StartPosition { get; }

        /// <summary>
        /// The source line number
        /// </summary>
        public int EndLine { get; }

        /// <summary>
        /// The first position within a source code line
        /// </summary>
        public int EndPosition { get; }
    }
}