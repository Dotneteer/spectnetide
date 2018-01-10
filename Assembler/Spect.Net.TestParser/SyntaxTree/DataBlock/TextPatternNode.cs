using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a text pattern
    /// </summary>
    public class TextPatternNode : MemoryPatternNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public TextPatternNode(ParserRuleContext context) : base(context)
        {
        }

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