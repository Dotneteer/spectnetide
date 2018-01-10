using System.Collections.Generic;
using Antlr4.Runtime;

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
        public TestOptionsNode(ParserRuleContext context) : base(context)
        {
            Options = new List<TestOptionNode>();
        }

        /// <summary>
        /// The 'with' keyword span
        /// </summary>
        public TextSpan WithKeywordSpan { get; set; }
        /// <summary>
        /// The list of test options
        /// </summary>
        public List<TestOptionNode> Options { get; }
    }
}