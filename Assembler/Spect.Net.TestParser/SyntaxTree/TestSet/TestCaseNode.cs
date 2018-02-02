using System;
using System.Collections.Generic;
using System.Text;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a test case node
    /// </summary>
    public class TestCaseNode : NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public TestCaseNode(Z80TestParser.TestCaseContext context) : base(context)
        {
            CaseKeywordSpan = new TextSpan(context.CASE());
            var sb = new StringBuilder(400);
            for (var i = 0; i < context.ChildCount; i++)
            {
                var token = context.GetChild(i).GetText();
                if (i > 0 && token != "," && token != ";") sb.Append(" ");
                sb.Append(token);
            }

            TestCaseText = sb.ToString();
            if (context.PORTMOCK() != null)
            {
                PortMockKeywordSpan = new TextSpan(context.PORTMOCK());
            }
            Expressions = new List<ExpressionNode>();
            PortMocks = new List<IdentifierNameNode>();
        }

        /// <summary>
        /// The 'case' span
        /// </summary>
        public TextSpan CaseKeywordSpan { get; }

        /// <summary>
        /// The text of the test case
        /// </summary>
        public string TestCaseText { get; }

        /// <summary>
        /// The test case expressions
        /// </summary>
        public List<ExpressionNode> Expressions { get; }

        /// <summary>
        /// The 'portmock' span
        /// </summary>
        public TextSpan? PortMockKeywordSpan { get; }

        /// <summary>
        /// The portmock identifiers
        /// </summary>
        public List<IdentifierNameNode> PortMocks { get; }
    }
}