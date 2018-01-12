using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a 'timeout' test option clause
    /// </summary>
    public class TimeoutTestOptionNode : TestOptionNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        /// <param name="expr">Timeout expression</param>
        public TimeoutTestOptionNode(Z80TestParser.TestOptionContext context, ExpressionNode expr) : base(context)
        {
            Expr = expr;
        }

        /// <summary>
        /// The timeout expression
        /// </summary>
        public ExpressionNode Expr { get; }
    }
}