using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a 'di' test option clause
    /// </summary>
    public class DiTestOptionNode : InterruptOptionNodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public DiTestOptionNode(ParserRuleContext context) : base(context)
        {
        }
    }
}