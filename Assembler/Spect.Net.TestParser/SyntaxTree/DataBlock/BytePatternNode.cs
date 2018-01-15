using System.Collections.Generic;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a byte pattern
    /// </summary>
    public class BytePatternNode : MemoryPatternNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public BytePatternNode(Z80TestParser.ByteSetContext context) : base(context)
        {
            ByteKeywordSpan = new TextSpan(context.BYTE());
            Bytes = new List<ExpressionNode>();
        }

        /// <summary>
        /// The 'byte' span
        /// </summary>
        public TextSpan ByteKeywordSpan { get; }

        /// <summary>
        /// Bytes of the pattern
        /// </summary>
        public List<ExpressionNode> Bytes { get; }
    }
}