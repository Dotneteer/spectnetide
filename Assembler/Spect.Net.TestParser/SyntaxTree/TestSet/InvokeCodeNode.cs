using Antlr4.Runtime;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents the act clause
    /// </summary>
    public class InvokeCodeNode: NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public InvokeCodeNode(ParserRuleContext context) : base(context)
        {
        }

        /// <summary>
        /// The span of the invoke keyword
        /// </summary>
        public TextSpan KeywordSpan { get; set; }

        /// <summary>
        /// Is a 'call' invocation?
        /// </summary>
        public bool IsCall { get; set; }

        /// <summary>
        /// The span of the 'call' or 'start' token
        /// </summary>
        public TextSpan CallOrStartSpan { get; set; }

        /// <summary>
        /// Start address expression
        /// </summary>
        public ExpressionNode StartExpr { get; set; }

        /// <summary>
        /// The span of the 'stop' or 'halt' token
        /// </summary>
        public TextSpan StopOrHaltSpan { get; set; }

        /// <summary>
        /// Is a 'stop' completion?
        /// </summary>
        public bool IsStop { get; set; }

        /// <summary>
        /// Stop address expression
        /// </summary>
        public ExpressionNode StopExpr { get; set; }
    }
}