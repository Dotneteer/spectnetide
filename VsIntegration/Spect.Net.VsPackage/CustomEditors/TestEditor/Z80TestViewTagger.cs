using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    public class Z80TestViewTagger : ITagger<TextMarkerTag>
    {
        public SpectNetPackage Package => SpectNetPackage.Default;
        public ITextBuffer SourceBuffer { get; }
        public ITextView View { get; }
        public string FilePath { get; }

        public Z80TestViewTagger(ITextBuffer buffer, ITextView textView, string filePath)
        {
            SourceBuffer = buffer;
            View = textView;
            FilePath = filePath;
        }

        /// <summary>Gets all the tags that intersect the specified spans. </summary>
        /// <returns>A <see cref="T:Microsoft.VisualStudio.Text.Tagging.TagSpan`1" /> for each tag.</returns>
        /// <param name="spans">The spans to visit.</param>
        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            yield break;
        }

        public void UpdateView(SnapshotPoint snapshotPoint)
        {
            View.DisplayTextLineContainingBufferPosition(snapshotPoint, View.ViewportTop, 0);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}