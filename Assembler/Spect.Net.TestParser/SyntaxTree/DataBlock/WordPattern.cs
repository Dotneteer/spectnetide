using System.Collections.Generic;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a byte pattern
    /// </summary>
    public class WordPattern : MemoryPattern
    {
        /// <summary>
        /// Words of the pattern
        /// </summary>
        public List<ExpressionNode> Words { get; }

        public WordPattern()
        {
            Words = new List<ExpressionNode>();
        }
    }
}