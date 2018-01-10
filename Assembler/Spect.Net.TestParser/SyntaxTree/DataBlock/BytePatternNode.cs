using System.Collections.Generic;
using Antlr4.Runtime;
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
        public BytePatternNode(ParserRuleContext context) : base(context)
        {
            Bytes = new List<ExpressionNode>();
        }

        /// <summary>
        /// Bytes of the pattern
        /// </summary>
        public List<ExpressionNode> Bytes { get; }
    }
}