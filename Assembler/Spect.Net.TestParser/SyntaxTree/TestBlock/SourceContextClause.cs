using System.Collections.Generic;

namespace Spect.Net.TestParser.SyntaxTree.TestBlock
{
    /// <summary>
    /// Represents a source context clause
    /// </summary>
    public class SourceContextClause: ClauseBase
    {
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
        public List<IdentifierClause> Symbols { get; }

        public SourceContextClause()
        {
            Symbols = new List<IdentifierClause>();
        }
    }
}