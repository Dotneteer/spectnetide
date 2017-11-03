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
    [TagType(typeof(Z80DebugTokenTag))]
    public class Z80DebugTaggerProvider: IViewTaggerProvider
    {
        /// <summary>
        /// The service that maintains the collection of all known classification types.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry;

        /// <summary>
        /// Creates an ITagAggregator{T} for an ITextBuffer.
        /// </summary>
        [Import]
        internal IViewTagAggregatorFactoryService AggregatorFactory;

        /// <summary>
        /// Allow to access the current package information from the tagger
        /// </summary>
        [Import(typeof(IHostPackageProvider))]
        internal IHostPackageProvider HostPackageProvider;

        /// <summary>
        /// Creates a tag provider for the specified view and buffer.
        /// </summary>
        /// <param name="textView">
        /// The <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextView" />.
        /// </param>
        /// <param name="buffer">
        /// The <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer" />.
        /// </param>
        /// <typeparam name="T">The type of the tag.</typeparam>
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
            var tagger = new Z80DebugTokenTagger(HostPackageProvider.Package, buffer, textView, filePath);
            HostPackageProvider.Package.DebugInfoProvider?.RegisterTagger(filePath, tagger);
            return tagger as ITagger<T>;
        }
    }

#pragma warning restore 649
}