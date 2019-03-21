using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 649

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    /// <summary>
    /// Tagger provider for the Z80 Assembly editor
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [ContentType("z80Test")]
    [TagType(typeof(Z80TestTokenTag))]
    public class Z80TestTaggerProvider : ITaggerProvider
    {
        /// <summary>
        /// Creates a tag provider for the specified buffer.
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="buffer">The <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer" />.</param>
        /// <returns>The tagger we use to create Z80 tast language tags</returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            string filePath = null;
            if (buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument docProperty))
            {
                filePath = docProperty.FilePath;
            }
            var tagger = new Z80TestTokenTagger(buffer, filePath);
            return tagger as ITagger<T>;
        }
    }
}

#pragma warning restore 649
