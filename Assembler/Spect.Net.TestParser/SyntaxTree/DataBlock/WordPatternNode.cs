using System.Collections.Generic;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a byte pattern
    /// </summary>
    public class WordPatternNode : MemoryPatternNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public WordPatternNode(Z80TestParser.WordSetContext context) : base(context)
        {
            WordKeywordSpan = new TextSpan(context.WORD());
            Words = new List<ExpressionNode>();
        }

        /// <summary>
        /// The 'word' span
        /// </summary>
        public TextSpan WordKeywordSpan { get; }

        /// <summary>
        /// Words of the pattern
        /// </summary>
        public List<ExpressionNode> Words { get; }
    }
}