namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// This class represents a text span with an inclusive start and
    /// an exclusive end position.
    /// </summary>
    public class TextSpan
    {
        /// <summary>
        /// Inclusive start position
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Exclusive end position
        /// </summary>
        public int End { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TextSpan(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}