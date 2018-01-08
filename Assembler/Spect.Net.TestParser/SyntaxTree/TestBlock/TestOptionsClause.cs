using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestBlock
{
    /// <summary>
    /// Represents the test options clause
    /// </summary>
    public class TestOptionsClause: ClauseBase
    {
        /// <summary>
        /// The 'with' keyword span
        /// </summary>
        public TextSpan WithKeywordSpan { get; set; }

        /// <summary>
        /// The 'nonmi' keyword span
        /// </summary>
        public TextSpan? NoNmiKeywordSpan { get; set; }

        /// <summary>
        /// The 'timeout' keyword span
        /// </summary>
        public TextSpan? TimeoutKeywordSpan { get; set; }

        /// <summary>
        /// The timeout expression
        /// </summary>
        public ExpressionNode Expr { get; set; }
    }
}