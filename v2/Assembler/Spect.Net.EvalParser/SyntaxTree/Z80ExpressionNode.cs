namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// Represents a Z80 expression node
    /// </summary>
    public class Z80ExpressionNode
    {
        /// <summary>
        /// Expression part
        /// </summary>
        public ExpressionNode Expression { get; }

        /// <summary>
        /// Format specifier part
        /// </summary>
        public FormatSpecifierNode FormatSpecifier { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80ExpressionNode(ExpressionNode expression, FormatSpecifierNode formatSpecifier)
        {
            Expression = expression;
            FormatSpecifier = formatSpecifier;
        }
    }
}