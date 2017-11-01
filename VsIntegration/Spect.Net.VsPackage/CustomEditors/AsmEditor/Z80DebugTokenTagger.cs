using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

#pragma warning disable 649
#pragma warning disable 67

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    public class Z80DebugTokenTagger: ITagger<Z80DebugTokenTag>
    {
        private ITextView View { get; }
        private ITextBuffer SourceBuffer { get; }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Creates the tagger with the specified view and source buffer
        /// </summary>
        /// <param name="view">The view to respond to</param>
        /// <param name="sourceBuffer">Source text</param>
        public Z80DebugTokenTagger(ITextView view, ITextBuffer sourceBuffer)
        {
            View = view;
            SourceBuffer = sourceBuffer;
            View.LayoutChanged += ViewLayoutChanged;
        }

        /// <summary>
        /// Gets all the tags that intersect the specified spans.
        /// </summary>
        /// <param name="spans">The spans to visit.</param>
        /// <returns>A <see cref="T:Microsoft.VisualStudio.Text.Tagging.TagSpan`1" /> for each tag.</returns>
        public IEnumerable<ITagSpan<Z80DebugTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var curSpan in spans)
            {
                var currentLine = curSpan.Start.GetContainingLine();
                if (currentLine.LineNumber == 3)
                {
                    yield return CreateSpan(currentLine);
                }
            }
        }

        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // --- If a new snapshot wasn't generated, then skip this layout
            if (e.NewViewState.EditSnapshot != e.OldViewState.EditSnapshot)
            {
                UpdateLayout();
            }
        }

        /// <summary>
        /// Updates the layout with breakpoints
        /// </summary>
        private void UpdateLayout()
        {
            var tempEvent = TagsChanged;
            tempEvent?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
        }

        /// <summary>
        /// Creates a snaphot span from an other snapshot span and a text span
        /// </summary>
        private static TagSpan<Z80DebugTokenTag> CreateSpan(ITextSnapshotLine line)
        {
            var tagSpan = new Span(line.Start.Position, 8);
            var span = new SnapshotSpan(line.Snapshot, tagSpan);
            var tag = new Z80DebugTokenTag("Z80Breakpoint");
            return new TagSpan<Z80DebugTokenTag>(span, tag);
        }


    }

#pragma warning restore 67
#pragma warning restore 649

}