using System.Collections.Generic;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents assertions
    /// </summary>
    public class AssertNode : NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public AssertNode(Z80TestParser.AssertContext context) : base(context)
        {
            AssertKeywordSpan = new TextSpan(context.ASSERT());
            Expressions = new List<ExpressionNode>();
        }

        /// <summary>
        /// The 'assert' span
        /// </summary>
        public TextSpan AssertKeywordSpan { get; }

        /// <summary>
        /// The assert expressions
        /// </summary>
        public List<ExpressionNode> Expressions { get; }
    }
}