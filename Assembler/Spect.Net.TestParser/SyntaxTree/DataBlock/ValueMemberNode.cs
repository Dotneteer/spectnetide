using Antlr4.Runtime;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a value member
    /// </summary>
    public class ValueMemberNode : DataMemberNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public ValueMemberNode(ParserRuleContext context) : base(context)
        {
        }

        /// <summary>
        /// Expression of the value member
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}