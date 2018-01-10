using System.Collections.Generic;
using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// This class represents a data block of the language
    /// </summary>
    public class DataBlockNode : NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public DataBlockNode(ParserRuleContext context) : base(context)
        {
            DataMembers = new List<DataMemberNode>();
        }

        /// <summary>
        /// The 'data' keyword span
        /// </summary>
        public TextSpan DataKeywordSpan { get; set; }

        /// <summary>
        /// The data members in this block
        /// </summary>
        public List<DataMemberNode> DataMembers { get; set; }
    }
}