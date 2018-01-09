using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a value member
    /// </summary>
    public class ValueMember : DataMember
    {
        /// <summary>
        /// Expression of the value member
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}