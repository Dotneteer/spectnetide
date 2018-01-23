using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

#pragma warning disable 649
#pragma warning disable 67

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    public class Z80TestViewTagger : ITagger<TextMarkerTag>
    {
        private static readonly Dictionary<string, Z80TestViewTagger> s_Taggers 
            = new Dictionary<string, Z80TestViewTagger>();

        public SpectNetPackage Package => SpectNetPackage.Default;
        public ITextBuffer SourceBuffer { get; }
        public ITextView View { get; }
        public string FilePath { get; }

        public Z80TestViewTagger(ITextBuffer buffer, ITextView textView, string filePath)
        {
            SourceBuffer = buffer;
            View = textView;
            FilePath = filePath;
            s_Taggers[filePath] = this;
        }

        /// <summary>Gets all the tags that intersect the specified spans. </summary>
        /// <returns>A <see cref="T:Microsoft.VisualStudio.Text.Tagging.TagSpan`1" /> for each tag.</returns>
        /// <param name="spans">The spans to visit.</param>
        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // --- We do not create any tags
            yield break;
        }

        /// <summary>
        /// Retrieves the first and last line index of the text buffer 
        /// that are in the current view
        /// </summary>
        private (int firstLine, int lastLine) GetViewPortLines()
        {
            var firstLine = (int) (View.ViewportTop / View.LineHeight);
            var lastLine = (int) ((View.ViewportTop + View.ViewportHeight) / View.LineHeight);
            return (firstLine, lastLine);
        }

        /// <summary>
        /// Retrieves the first and last line index of the specified text buffer 
        /// that are in the current view
        /// </summary>
        /// <param name="filePath">Text file path</param>
        public static (int firstLine, int lastLine) GetViewPortLines(string filePath)
        {
            return s_Taggers.TryGetValue(filePath, out var tagger) 
                ? tagger.GetViewPortLines() 
                : (-1, -1);
        }

        /// <summary>
        /// Occurs when tags are added to or removed from the provider.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}

#pragma warning restore 67
#pragma warning restore 649
