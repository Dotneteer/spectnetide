using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a port pulse node
    /// </summary>
    public class PortPulseNode : NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public PortPulseNode(Z80TestParser.PortPulseContext context) : base(context)
        {
        }

        /// <summary>
        /// Mocked port value
        /// </summary>
        public ExpressionNode ValueExpr { get; set; }

        /// <summary>
        /// Pulse1 tact
        /// </summary>
        public ExpressionNode Pulse1Expr { get; set; }

        /// <summary>
        /// Pulse2 tact
        /// </summary>
        public ExpressionNode Pulse2Expr { get; set; }

        /// <summary>
        /// Indicates if Pulse1 and Pulse2 values define an interval
        /// </summary>
        public bool IsInterval { get; set; }
    }
}