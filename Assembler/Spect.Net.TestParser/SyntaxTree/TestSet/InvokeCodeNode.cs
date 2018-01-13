using Spect.Net.TestParser.Generated;
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
        /// <param name="startExpr">Start expression</param>
        /// <param name="stopExpr">Stop expression</param>
        public InvokeCodeNode(Z80TestParser.InvokeCodeContext context, ExpressionNode startExpr,
            ExpressionNode stopExpr) : base(context)
        {
            StartExpr = startExpr;
            if (context.CALL() != null)
            {
                IsCall = true;
                CallOrStartSpan = new TextSpan(context.CALL());
                return;
            }

            IsCall = false;
            CallOrStartSpan = new TextSpan(context.START());
            if (context.STOP() != null)
            {
                IsHalt = false;
                StopOrHaltSpan = new TextSpan(context.STOP());
                StopExpr = stopExpr;
                return;
            }

            if (context.HALT() != null)
            {
                IsHalt = true;
                StopOrHaltSpan = new TextSpan(context.HALT());
            }
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
        /// Is a 'halt' completion?
        /// </summary>
        public bool IsHalt { get; set; }

        /// <summary>
        /// Stop address expression
        /// </summary>
        public ExpressionNode StopExpr { get; set; }
    }
}