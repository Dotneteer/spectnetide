using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents an abstract interrupt test option clause
    /// </summary>
    public abstract class InterruptOptionNodeBase : TestOptionNodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        protected InterruptOptionNodeBase(ParserRuleContext context) : base(context)
        {
        }
    }
}