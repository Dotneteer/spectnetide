using Antlr4.Runtime;
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
        public TimeoutTestOptionNode(ParserRuleContext context) : base(context)
        {
        }

        /// <summary>
        /// The timeout expression
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}