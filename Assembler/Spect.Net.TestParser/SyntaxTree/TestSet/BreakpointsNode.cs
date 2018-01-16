using System.Collections.Generic;
using Antlr4.Runtime;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents the collection of breakpoints
    /// </summary>
    public class BreakpointsNode : NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public BreakpointsNode(Z80TestParser.BreakpointContext context) : base(context)
        {
            BreakpointKeywordSpan = new TextSpan(context.BREAKPOINT());
            Expressions = new List<ExpressionNode>();
        }

        /// <summary>
        /// The 'breakpoint' span
        /// </summary>
        public TextSpan BreakpointKeywordSpan { get; }

        /// <summary>
        /// The breakpoint expressions
        /// </summary>
        public List<ExpressionNode> Expressions { get; }
    }
}