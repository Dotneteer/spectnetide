using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 649
#pragma warning disable 67

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("z80Asm")]
    [TagType(typeof(ClassificationTag))]
    public class Z80AsmClassifierProvider : ITaggerProvider
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
        /// <returns>
        /// The <see cref="T:Microsoft.VisualStudio.Text.Tagging.ITagger`1" />.
        /// </returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var z80AsmTagAggregator = AggregatorFactory.CreateTagAggregator<Z80AsmTokenTag>(buffer);
            return new Z80AsmClassifier(z80AsmTagAggregator, _classificationRegistry) as ITagger<T>;
        }
    }

#pragma warning restore 67
#pragma warning restore 649
}