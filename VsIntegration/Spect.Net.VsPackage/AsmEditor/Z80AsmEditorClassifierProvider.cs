using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
#pragma warning disable 649

namespace Spect.Net.VsPackage.AsmEditor
{
    /// <summary>
    /// Classifier provider for the Z80 Assembly editor
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("z80Asm")] 
    internal class Z80AsmEditorClassifierProvider : ITaggerProvider
    {
        /// <summary>
        /// The content type of the Z80 assembly editor
        /// </summary>
        [Export]
        [Name("z80Asm")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition Z80ContentType;

        /// <summary>
        /// We associate the Z80 assembly content type with the .z80Asm extension
        /// </summary>
        [Export]
        [FileExtension(".z80Asm")]
        [ContentType("z80Asm")]
        internal static FileExtensionToContentTypeDefinition Z80AsmFileType;

        /// <summary>
        /// The service that maintains the collection of all known classification types.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry;

        /// <summary>
        /// Creates an ITagAggregator{T} for an ITextBuffer.
        /// </summary>
        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory;

        /// <summary>
        /// Classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        private IClassificationTypeRegistryService _classificationRegistry;

        /// <summary>
        /// Creates a tag provider for the specified buffer.
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="buffer">The <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer" />.</param>
        /// <returns>The tagger we use to create Z80 assembly tags</returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            throw new System.NotImplementedException();
        }
    }

#pragma warning restore 649
}
