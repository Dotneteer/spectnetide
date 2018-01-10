using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents an assignment
    /// </summary>
    public abstract class AssignmentNode: NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        protected AssignmentNode(ParserRuleContext context) : base(context)
        {
        }
    }
}