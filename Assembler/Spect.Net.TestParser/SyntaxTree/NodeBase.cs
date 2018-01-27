using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree
{
    /// <summary>
    /// This class represents a single Z80 Test language clause
    /// </summary>
    public abstract class NodeBase
    {
        /// <summary>
        /// Te span of the clause
        /// </summary>
        public TextSpan Span { get; set; }

        /// <summary>
        /// The index of the source file this language block belongs to
        /// </summary>
        public int FileIndex { get; set; }

        /// <summary>
        /// Initializes an empty clause
        /// </summary>
        protected NodeBase()
        {
        }

        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        protected NodeBase(ParserRuleContext context)
        {
            Span = new TextSpan(context);
        }
    }
}