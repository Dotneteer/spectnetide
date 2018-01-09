using System.Collections.Generic;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a byte pattern
    /// </summary>
    public class BytePattern : MemoryPattern
    {
        /// <summary>
        /// Bytes of the pattern
        /// </summary>
        public List<ExpressionNode> Bytes { get; }

        public BytePattern()
        {
            Bytes = new List<ExpressionNode>();
        }
    }
}