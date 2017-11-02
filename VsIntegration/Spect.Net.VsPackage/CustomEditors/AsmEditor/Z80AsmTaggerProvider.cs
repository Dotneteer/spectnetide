using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 649

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// Tagger provider for the Z80 Assembly editor
    /// </summary>
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("z80Asm")]
    [TagType(typeof(Z80AsmTokenTag))]
    internal class Z80AsmTaggerProvider : IViewTaggerProvider
    {
        /// <summary>
        /// The service that maintains the collection of all known classification types.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry;

        [Import(typeof(IHostPackageProvider))]
        internal IHostPackageProvider HostPackageProvider;

        /// <summary>
        /// Creates a tag provider for the specified view and buffer.
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="textView">
        /// The <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />.
        /// </param>
        /// <param name="buffer">
        /// The <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer" />.
        /// </param>
        /// <returns>
        /// The <see cref="T:Microsoft.VisualStudio.Text.Tagging.ITagAggregator`1" /> 
        /// of the correct type for <paramref name="textView" />.
        /// </returns>
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            string filePath = null;
            if (buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument docProperty))
            {
                filePath = docProperty.FilePath;
            }
            var tagger = new Z80AsmTokenTagger(HostPackageProvider.Package, textView, buffer, filePath);
            HostPackageProvider.Package.DebugInfoProvider?.RegisterTagger(filePath, tagger);
            return tagger as ITagger<T>;
        }
    }

#pragma warning restore 649
}
