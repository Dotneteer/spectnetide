using Spect.Net.TestParser.Generated;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a machine context
    /// </summary>
    public class MachineContextNode: NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public MachineContextNode(Z80TestParser.MachineContextContext context) : base(context)
        {
            MachineKeywordSpan = new TextSpan(context.MACHINE());
            IdSpan = new TextSpan(context.IDENTIFIER());
            Id = context.IDENTIFIER()?.GetText();
        }

        /// <summary>
        /// The 'machine' span
        /// </summary>
        public TextSpan MachineKeywordSpan { get; }

        /// <summary>
        /// The identifier span
        /// </summary>
        public TextSpan IdSpan { get; }

        /// <summary>
        /// The id value
        /// </summary>
        public string Id { get; }
    }
}