using System.Collections.Generic;
using Antlr4.Runtime;
using Spect.Net.TestParser.Generated;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a memory pattern member
    /// </summary>
    public class MemoryPatternMemberNode : DataMemberNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public MemoryPatternMemberNode(Z80TestParser.MemPatternContext context) : base(context)
        {
            IdSpan = new TextSpan(context.IDENTIFIER());
            Id = context.IDENTIFIER()?.GetText();
            Patterns = new List<MemoryPatternNode>();
        }

        /// <summary>
        /// Patterns in 'data'
        /// </summary>
        public List<MemoryPatternNode> Patterns { get; }
    }
}