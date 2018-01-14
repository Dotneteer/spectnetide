using Spect.Net.TestParser.Generated;
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
        /// <param name="expr">Value node expression</param>
        public ValueMemberNode(Z80TestParser.ValueDefContext context, ExpressionNode expr) : base(context)
        {
            IdSpan = new TextSpan(context.IDENTIFIER().Symbol);
            Id = context.IDENTIFIER()?.GetText();
            Expr = expr;
        }

        /// <summary>
        /// Expression of the value member
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}