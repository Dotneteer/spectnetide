using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("z80Test")]
    [TagType(typeof(TextMarkerTag))]
    public class Z80TestViewTaggerProvider : IViewTaggerProvider
    {
        /// <summary>Creates a tag provider for the specified view and buffer.</summary>
        /// <returns>The <see cref="T:Microsoft.VisualStudio.Text.Tagging.ITagAggregator`1" /> of the correct type for <paramref name="textView" />.</returns>
        /// <param name="textView">The <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />.</param>
        /// <param name="buffer">The <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer" />.</param>
        /// <typeparam name="T">The type of the tag.</typeparam>
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            string filePath = null;
            if (buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument docProperty))
            {
                filePath = docProperty.FilePath;
            }
            var tagger = new Z80TestViewTagger(buffer, textView, filePath);
            return tagger as ITagger<T>;
        }
    }
}