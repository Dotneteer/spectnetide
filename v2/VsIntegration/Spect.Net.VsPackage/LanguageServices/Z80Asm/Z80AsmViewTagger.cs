using Antlr4.Runtime;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Spect.Net.Assembler;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spect.Net.VsPackage.LanguageServices.Z80Asm
{
    /// <summary>
    /// This tagger provides classification tags for the Z80 assembly 
    /// debug markers
    /// </summary>
    public class Z80AsmViewTagger : ITagger<TextMarkerTag>
    {
        public SpectNetPackage Package => SpectNetPackage.Default;
        public ITextBuffer SourceBuffer { get; }
        public ITextView View { get; }
        public string FilePath { get; }

        public Z80AsmViewTagger(ITextBuffer buffer, ITextView textView, string filePath)
        {
            SourceBuffer = buffer;
            View = textView;
            FilePath = filePath;
            View.LayoutChanged += ViewLayoutChanged;
        }

        /// <summary>
        /// Update the entire layout whenever the buffer's snapshot changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // --- If a new snapshot wasn't generated, then skip this layout
            if (e.NewViewState.EditSnapshot != e.OldViewState.EditSnapshot)
            {
                var tempEvent = TagsChanged;
                tempEvent?.Invoke(this, new SnapshotSpanEventArgs(
                    new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
            }
        }

        /// <summary>
        /// Updates the specified line number to display/undisplay current breakpoint marker
        /// </summary>
        /// <param name="lineNo">Line number</param>
        /// <param name="isCurrent">Is this the current breakpoint line?</param>
        public void UpdateLine(int lineNo)
        {
            var lines = View.VisualSnapshot.Lines;
            var line = lines.FirstOrDefault(a => a.LineNumber == lineNo);
            if (line == null) return;

            var startPosition = line.Start;
            var endPosition = line.EndIncludingLineBreak;

            var span = new SnapshotSpan(View.TextSnapshot, Span.FromBounds(startPosition, endPosition));
            if (View is IWpfTextView wpfTextView)
            {
                wpfTextView.ViewScroller.EnsureSpanVisible(span, EnsureSpanVisibleOptions.AlwaysCenter);
                var firstLine = wpfTextView.TextViewLines.FirstVisibleLine;
                var lastLine = wpfTextView.TextViewLines.LastVisibleLine;
                if (firstLine == null || lastLine == null) return;
                var newSpan = new SnapshotSpan(wpfTextView.TextSnapshot,
                    Span.FromBounds(firstLine.Start, lastLine.EndIncludingLineBreak));
                var tempEvent = TagsChanged;
                tempEvent?.Invoke(this, new SnapshotSpanEventArgs(
                    new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
            }
        }

        /// <summary>
        /// Gets all the tags that intersect the specified spans. 
        /// </summary>
        /// <param name="spans">The spans to visit.</param>
        /// <returns>
        /// A <see cref="T:Microsoft.VisualStudio.Text.Tagging.TagSpan`1" /> for each tag.
        /// </returns>
        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // --- Go through the tags
            foreach (var curSpan in spans)
            {
                var firstLineNo = curSpan.Start.GetContainingLine().LineNumber;
                var lastLineNo = curSpan.End.GetContainingLine().LineNumber;
                foreach (var line in SourceBuffer.CurrentSnapshot.Lines)
                {
                    if (line.LineNumber < firstLineNo || line.LineNumber > lastLineNo) continue;

                    var textOfLine = line.GetText();

                    // --- Let's use the Z80 assembly parser to obtain tags
                    var inputStream = new AntlrInputStream(textOfLine);
                    var lexer = new Z80AsmLexer(inputStream);
                    var tokenStream = new CommonTokenStream(lexer);
                    var parser = new Z80AsmParser(tokenStream);
                    var context = parser.asmline();
                    var visitor = new Z80AsmVisitor(inputStream);
                    visitor.Visit(context);
                    if (!(visitor.LastAsmLine is SourceLineBase asmline)) continue;

                    var package = SpectNetPackage.Default;
                    if (package != null 
                        && package.DebugInfoProvider.CurrentBreakpointFile == FilePath
                        && package.DebugInfoProvider.CurrentBreakpointLine - 1 == line.LineNumber)
                    {
                        // --- Check for the current breakpoint
                        yield return CreateSpan(line,
                            Package.Options.FullLineHighlight
                                ? new TextSpan(0, textOfLine.Length) : asmline.InstructionSpan,
                            Z80AsmClassificationTypes.Z80_CURRENT_BREAKPOINT);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a snaphot span from an other snapshot span and a text span
        /// </summary>
        private static TagSpan<TextMarkerTag> CreateSpan(ITextSnapshotLine line,
            TextSpan text, string type)
        {
            var tagSpan = new Span(line.Start.Position + text.Start, text.End - text.Start);
            var span = new SnapshotSpan(line.Snapshot, tagSpan);
            var tag = new TextMarkerTag(type);
            return new TagSpan<TextMarkerTag>(span, tag);
        }


        /// <summary>
        /// Occurs when tags are added to or removed from the provider.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
