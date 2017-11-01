using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("z80Asm")]
    [TagType(typeof(ClassificationTag))]
    public class Z80DebugClassifierProvider : ITaggerProvider
    {
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
            var z80DebugTagAggregator = AggregatorFactory.CreateTagAggregator<Z80DebugTokenTag>(buffer);
            return new Z80DebugClassifier(z80DebugTagAggregator, _classificationRegistry) as ITagger<T>;
        }
    }
}