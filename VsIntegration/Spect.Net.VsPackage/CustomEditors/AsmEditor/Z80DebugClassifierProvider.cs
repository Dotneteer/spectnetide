using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 649
#pragma warning disable 67

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("z80Asm")]
    [TagType(typeof(ClassificationTag))]
    public class Z80DebugClassifierProvider: IViewTaggerProvider
    {
        /// <summary>
        /// Creates an ITagAggregator{T} for an ITextBuffer.
        /// </summary>
        [Import]
        internal IViewTagAggregatorFactoryService AggregatorFactory;

        /// <summary>
        /// Classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        private IClassificationTypeRegistryService _classificationRegistry;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            var z80AsmTagAggregator = AggregatorFactory.CreateTagAggregator<Z80DebugTokenTag>(textView);
            return new Z80DebugClassifier(z80AsmTagAggregator, _classificationRegistry) as ITagger<T>;
        }
    }

#pragma warning restore 67
#pragma warning restore 649
}