using System.Collections.Generic;
using Spect.Net.TestParser.Generated;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents test parameters
    /// </summary>
    public class ParamsNode : NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public ParamsNode(Z80TestParser.TestParamsContext context) : base(context)
        {
            ParamsKeywordSpan = new TextSpan(context.PARAMS());
            Ids = new List<IdentifierNameNode>();
        }

        /// <summary>
        /// The 'params' span
        /// </summary>
        public TextSpan ParamsKeywordSpan { get; }

        /// <summary>
        /// The parameter IDs
        /// </summary>
        public List<IdentifierNameNode> Ids { get; }
    }
}