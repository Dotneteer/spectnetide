using System.Collections.Generic;
using Spect.Net.TestParser.Generated;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents the test options clause
    /// </summary>
    public class TestOptionsNode: NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public TestOptionsNode(Z80TestParser.TestOptionsContext context) : base(context)
        {
            WithKeywordSpan = new TextSpan(context.WITH());
            Options = new List<TestOptionNode>();
        }

        /// <summary>
        /// The 'with' keyword span
        /// </summary>
        public TextSpan WithKeywordSpan { get; }

        /// <summary>
        /// The list of test options
        /// </summary>
        public List<TestOptionNode> Options { get; }
    }
}