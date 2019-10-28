namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// Represents a format specified 
    /// </summary>
    public class FormatSpecifierNode
    {
        /// <summary>
        /// Format specifier value
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public FormatSpecifierNode(string format)
        {
            Format = format.Substring(1);
        }
    }
}