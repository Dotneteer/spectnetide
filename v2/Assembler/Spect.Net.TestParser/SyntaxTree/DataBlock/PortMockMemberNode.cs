using System.Collections.Generic;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a port mock member
    /// </summary>
    public class PortMockMemberNode : DataMemberNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        /// <param name="expr">Port value expression</param>
        public PortMockMemberNode(Z80TestParser.PortMockContext context, ExpressionNode expr) : base(context)
        {
            IdSpan = new TextSpan(context.IDENTIFIER());
            Id = context.IDENTIFIER()?.GetText();
            Expr = expr;
            Pulses = new List<PortPulseNode>();
        }

        /// <summary>
        /// Port number expression
        /// </summary>
        public ExpressionNode Expr { get; set; }

        /// <summary>
        /// Represents the mocked pulses
        /// </summary>
        public List<PortPulseNode> Pulses { get; }
    }
}