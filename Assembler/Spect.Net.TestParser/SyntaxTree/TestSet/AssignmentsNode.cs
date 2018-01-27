using System.Collections.Generic;
using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents the arrange clause
    /// </summary>
    public class AssignmentsNode: NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public AssignmentsNode(ParserRuleContext context) : base(context)
        {
            KeywordSpan = context.CreateSpan(0);
            Assignments = new List<AssignmentNode>();
        }

        /// <summary>
        /// The assignment keyword span
        /// </summary>
        public TextSpan KeywordSpan { get; }

        /// <summary>
        /// Assignments within this node
        /// </summary>
        public List<AssignmentNode> Assignments { get; }
    }
}