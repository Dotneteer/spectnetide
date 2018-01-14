using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 649
#pragma warning disable 67

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("z80Test")]
    [TagType(typeof(ClassificationTag))]
    public class Z80TestClassifierProvider : ITaggerProvider
    {
        /// <summary>
        /// The content type of the Z80 test language editor
        /// </summary>
        [Export]
        [Name("z80Test")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition Z80ContentType;

        /// <summary>
        /// We associate the Z80 test content type with the .z80test extension
        /// </summary>
        [Export]
        [FileExtension(".z80test")]
        [ContentType("z80Test")]
        internal static FileExtensionToContentTypeDefinition Z80TestFileType;


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
            var z80TestTagAggregator = AggregatorFactory.CreateTagAggregator<Z80TestTokenTag>(buffer);
            return new Z80TestClassifier(z80TestTagAggregator, _classificationRegistry) as ITagger<T>;
        }
    }
}

#pragma warning restore 67
#pragma warning restore 649
