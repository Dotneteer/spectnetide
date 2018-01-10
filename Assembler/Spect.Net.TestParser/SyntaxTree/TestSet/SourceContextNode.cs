using System.Collections.Generic;
using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a source context clause
    /// </summary>
    public class SourceContextNode: NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public SourceContextNode(ParserRuleContext context) : base(context)
        {
            Symbols = new List<IdentifierNameNode>();
        }

        /// <summary>
        /// The 'source' keyword span
        /// </summary>
        public TextSpan SourceKeywordSpan { get; set; }

        /// <summary>
        /// The source file
        /// </summary>
        public string SourceFile { get; set; }

        /// <summary>
        /// The 'source' keyword span
        /// </summary>
        public TextSpan? SymbolsKeywordSpan { get; set; }

        /// <summary>
        /// The list of predefined symbols
        /// </summary>
        public List<IdentifierNameNode> Symbols { get; }
    }
}