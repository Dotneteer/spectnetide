using System.Collections.Generic;
using Spect.Net.TestParser.SyntaxTree.TestSet;

namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// This class represents a compilation unit
    /// </summary>
    public class CompilationUnit: NodeBase
    {
        /// <summary>
        /// The language blocks
        /// </summary>
        public List<TestSetNode> TestSets { get; }

        public CompilationUnit()
        {
            TestSets = new List<TestSetNode>();
        }
    }
}