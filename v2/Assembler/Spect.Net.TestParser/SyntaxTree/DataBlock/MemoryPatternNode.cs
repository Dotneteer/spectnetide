using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents an abstract memory pattern
    /// </summary>
    public abstract class MemoryPatternNode: NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        protected MemoryPatternNode(ParserRuleContext context) : base(context)
        {
        }

        /// <summary>
        /// The 'keyword' span of the clause
        /// </summary>
        public TextSpan KeywordSpan { get; set; }
    }
}