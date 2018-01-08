namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// This class represents a single Z80 Test language clause
    /// </summary>
    public abstract class ClauseBase
    {
        /// <summary>
        /// Te span of the clause
        /// </summary>
        public TextSpan Span { get; set; }
    }
}