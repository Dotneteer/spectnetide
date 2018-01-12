using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents an identifier
    /// </summary>
    public class IdentifierNameNode: NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        /// <param name="childIndex">Identifier child index</param>
        public IdentifierNameNode(ParserRuleContext context, int childIndex) 
        {
            Span = context.CreateSpan(childIndex);
            Id = context.GetTokenText(childIndex);
        }

        /// <summary>
        /// The ID 
        /// </summary>
        public string Id { get; }
    }
}