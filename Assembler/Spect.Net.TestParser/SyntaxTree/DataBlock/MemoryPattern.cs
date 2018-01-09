namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents an abstract memory pattern
    /// </summary>
    public abstract class MemoryPattern: ClauseBase
    {
        /// <summary>
        /// The 'keyword' span of the clause
        /// </summary>
        public TextSpan KeywordSpan { get; set; }
    }
}