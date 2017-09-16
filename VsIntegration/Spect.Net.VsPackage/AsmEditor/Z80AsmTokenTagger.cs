using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Spect.Net.Assembler;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;

#pragma warning disable 649
#pragma warning disable 67

namespace Spect.Net.VsPackage.AsmEditor
{
    internal sealed class Z80AsmTokenTagger: ITagger<Z80AsmTokenTag>
    {
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
                var currentLine = curSpan.Start.GetContainingLine();
                var textOfLine = currentLine.GetText();

                // --- Let's use the Z80 assembly parser to obtain tags
                var inputStream = new AntlrInputStream(textOfLine);
                var lexer = new Z80AsmLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new Z80AsmParser(tokenStream);
                var context = parser.asmline();
                var visitor = new Z80AsmVisitor();
                visitor.Visit(context);
                if (!(visitor.LastAsmLine is SourceLineBase asmline)) continue;

                if (asmline.LabelSpan != null)
                {
                    // --- Retrieve a label
                    yield return CreateSpan(currentLine, asmline.LabelSpan, Z80AsmTokenType.Label);
                }

                if (asmline.KeywordSpan != null)
                {
                    var type = Z80AsmTokenType.Instruction;
                    if (asmline is PragmaBase)
                    {
                        type = Z80AsmTokenType.Pragma;
                    }
                    else if (asmline is Directive)
                    {
                        type = Z80AsmTokenType.Directive;
                    }

                    // --- Retrieve a pragma/directive/instruction
                    yield return CreateSpan(currentLine, asmline.KeywordSpan, type);
                }

                if (asmline.CommentSpan != null)
                {
                    // --- Retrieve a comment
                    yield return CreateSpan(currentLine, asmline.CommentSpan, Z80AsmTokenType.Comment);
                }

                if (asmline.Numbers != null)
                {
                    foreach (var numberSpan in asmline.Numbers)
                    {
                        // --- Retrieve a number
                        yield return CreateSpan(currentLine, numberSpan, Z80AsmTokenType.Number);
                    }
                }

                if (asmline.Identifiers == null) continue;

                foreach (var idSpan in asmline.Identifiers)
                {
                    // --- Retrieve a number
                    yield return CreateSpan(currentLine, idSpan, Z80AsmTokenType.Identifier);
                }
            }
        }

        /// <summary>
        /// Creates a snaphot span from an other snapshot span and a text span
        /// </summary>
        private static TagSpan<Z80AsmTokenTag> CreateSpan(ITextSnapshotLine line, 
            TextSpan text, Z80AsmTokenType tokenType)
        {
            var tagSpan = new Span(line.Start.Position + text.Start, text.End - text.Start);
            var span = new SnapshotSpan(line.Snapshot, tagSpan);
            var tag = new Z80AsmTokenTag(tokenType);
            return new TagSpan<Z80AsmTokenTag>(span, tag);
        }

    }

#pragma warning restore 67
#pragma warning restore 649

}