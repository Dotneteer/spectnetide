using Spect.Net.TestParser.Generated;

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
        public NoNmiTestOptionNode(Z80TestParser.TestOptionContext context) : base(context)
        {
        }
    }
}