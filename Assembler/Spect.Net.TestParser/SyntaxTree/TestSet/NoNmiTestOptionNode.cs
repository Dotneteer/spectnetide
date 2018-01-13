using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a 'nonmi' test option clause
    /// </summary>
    public class NoNmiTestOptionNode : TestOptionNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public NoNmiTestOptionNode(ParserRuleContext context) : base(context)
        {
        }
    }
}