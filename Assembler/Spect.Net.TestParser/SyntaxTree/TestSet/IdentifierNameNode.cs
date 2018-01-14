using Antlr4.Runtime.Tree;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents an identifier
    /// </summary>
    public class IdentifierNameNode: NodeBase
    {
        /// <summary>
        /// Creates a node from the specified teminal node
        /// </summary>
        /// <param name="term"></param>
        public IdentifierNameNode(IParseTree term)
        {
            Span = new TextSpan(term);
            Id = term?.GetText();
        }

        /// <summary>
        /// The ID 
        /// </summary>
        public string Id { get; }
    }
}