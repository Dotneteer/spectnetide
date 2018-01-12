using System.Collections.Generic;
using Spect.Net.TestParser.Generated;

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
        public SourceContextNode(Z80TestParser.SourceContextContext context) : base(context)
        {
            SourceKeywordSpan = new TextSpan(context.SOURCE().Symbol);
            SourceFileSpan = new TextSpan(context.STRING().Symbol);
            SourceFile = context.STRING().GetText().Unquote();
            Symbols = new List<IdentifierNameNode>();
            if (context.ChildCount < 4) return;

            SymbolsKeywordSpan = new TextSpan(context.SYMBOLS().Symbol);
            var childIndex = 3;
            while (childIndex < context.ChildCount - 1)
            {
                Symbols.Add(new IdentifierNameNode(context, childIndex++));
            }
        }

        /// <summary>
        /// The 'source' keyword span
        /// </summary>
        public TextSpan SourceKeywordSpan { get; }

        /// <summary>
        /// The source file span
        /// </summary>
        public TextSpan SourceFileSpan { get; }

        /// <summary>
        /// The source file
        /// </summary>
        public string SourceFile { get; }

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