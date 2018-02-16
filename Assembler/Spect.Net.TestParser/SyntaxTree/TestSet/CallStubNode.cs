using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a call stub node
    /// </summary>
    public class CallStubNode : NodeBase
    {
        /// <summary>
        /// The 'callstab' keyword span
        /// </summary>
        public TextSpan CallStubKeywordSpan { get; }

        /// <summary>
        /// The value of the callstub
        /// </summary>
        public ExpressionNode Value { get; }

        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Callstub context</param>
        /// <param name="expr">Callstub expression</param>
        public CallStubNode(Z80TestParser.CallstubContext context, ExpressionNode expr) : base(context)
        {
            CallStubKeywordSpan = new TextSpan(context.CALLSTUB());
            Value = expr;
        }
    }
}