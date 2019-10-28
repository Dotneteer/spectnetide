using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a data member
    /// </summary>
    public abstract class DataMemberNode: NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        protected DataMemberNode(ParserRuleContext context) : base(context)
        {
        }

        /// <summary>
        /// The ID of the data member
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The span of the ID
        /// </summary>
        public TextSpan IdSpan { get; set; }
    }
}