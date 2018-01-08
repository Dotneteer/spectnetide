namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a data member
    /// </summary>
    public abstract class DataMember: ClauseBase
    {
        /// <summary>
        /// The ID of the data member
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The span of the ID
        /// </summary>
        public TextSpan IdSpan { get; set; }
    }
}