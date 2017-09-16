using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Spect.Net.Assembler;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.VsPackage.AsmEditor
{
    internal sealed class Z80AsmTokenTagger: ITagger<Z80AsmTokenTag>
    {
        private readonly ITextBuffer _buffer;
        private readonly Dictionary<string, Z80AsmTokenType> _tokenTypes = 
            new Dictionary<string, Z80AsmTokenType>();

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80AsmTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _tokenTypes.Add("Z80Label", Z80AsmTokenType.Label);
            _tokenTypes.Add("Z80Pragma", Z80AsmTokenType.Pragma);
            _tokenTypes.Add("Z80Directive", Z80AsmTokenType.Directive);
            _tokenTypes.Add("Z80Instruction", Z80AsmTokenType.Instruction);
            _tokenTypes.Add("Z80Comment", Z80AsmTokenType.Comment);
            _tokenTypes.Add("Z80Number", Z80AsmTokenType.Number);
            _tokenTypes.Add("Z80Identifier", Z80AsmTokenType.Identifier);
        }

        /// <summary>
        /// Occurs when tags are added to or removed from the provider.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Gets all the tags that intersect the specified spans.
        /// </summary>
        /// <param name="spans">The spans to visit.</param>
        /// <returns>
        /// A <see cref="T:Microsoft.VisualStudio.Text.Tagging.TagSpan`1" /> for each tag.
        /// </returns>
        public IEnumerable<ITagSpan<Z80AsmTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var curSpan in spans)
            {
                var textOfLine = curSpan.Start.GetContainingLine().GetText();

                // --- Let's use the Z80 assembly parser to obtain tags
                var inputStream = new AntlrInputStream(textOfLine);
                var lexer = new Z80AsmLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new Z80AsmParser(tokenStream);
                var context = parser.asmline();
                var visitor = new Z80AsmVisitor();
                if (!(visitor.Visit(context) is SourceLineBase asmline)) continue;

                yield break;
                //// --- We found a Z80 assembly line that may contain tokens

                //foreach (string ookToken in tokens)
                //{
                //    if (_ookTypes.ContainsKey(ookToken))
                //    {
                //        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, ookToken.Length));
                //        if (tokenSpan.IntersectsWith(curSpan))
                //            yield return new TagSpan<OokTokenTag>(tokenSpan,
                //                new OokTokenTag(_ookTypes[ookToken]));
                //    }
                //}
            }
        }

    }
}