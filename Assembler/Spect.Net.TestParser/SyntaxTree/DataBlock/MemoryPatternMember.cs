using System.Collections.Generic;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a memory pattern member
    /// </summary>
    public class MemoryPatternMember : DataMember
    {
        /// <summary>
        /// Patterns in 'data'
        /// </summary>
        public List<MemoryPattern> Patterns { get; }

        public MemoryPatternMember()
        {
            Patterns = new List<MemoryPattern>();
        }
    }
}