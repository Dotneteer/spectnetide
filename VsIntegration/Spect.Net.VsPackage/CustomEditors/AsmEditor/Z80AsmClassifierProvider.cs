using System;
using System.Collections.Generic;
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
    public class Z80AsmClassifierProvider : IViewTaggerProvider
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
        internal IViewTagAggregatorFactoryService AggregatorFactory;

        /// <summary>
        /// Classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        private IClassificationTypeRegistryService _classificationRegistry;

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
            var z80AsmTagAggregator = AggregatorFactory.CreateTagAggregator<Z80AsmTokenTag>(textView);
            return new Z80AsmClassifier(z80AsmTagAggregator, _classificationRegistry) as ITagger<T>;
        }
    }

    /// <summary>
    /// Classifier for the Z80assembly language
    /// </summary>
    public class Z80AsmClassifier : ITagger<ClassificationTag>
    {
        private readonly ITagAggregator<Z80AsmTokenTag> _aggregator;
        private readonly Dictionary<string, IClassificationType> _z80AsmTypes = 
            new Dictionary<string, IClassificationType>();

        public Z80AsmClassifier(ITagAggregator<Z80AsmTokenTag> aggregator,
            IClassificationTypeRegistryService typeService)
        {
            _aggregator = aggregator;
            _z80AsmTypes.Add(nameof(Z80AsmTokenType.Label), 
                typeService.GetClassificationType("Z80Label"));
            _z80AsmTypes.Add(nameof(Z80AsmTokenType.Pragma), 
                typeService.GetClassificationType("Z80Pragma"));
            _z80AsmTypes.Add(nameof(Z80AsmTokenType.Directive), 
                typeService.GetClassificationType("Z80Directive"));
            _z80AsmTypes.Add(nameof(Z80AsmTokenType.Instruction), 
                typeService.GetClassificationType("Z80Instruction"));
            _z80AsmTypes.Add(nameof(Z80AsmTokenType.Comment), 
                typeService.GetClassificationType("Z80Comment"));
            _z80AsmTypes.Add(nameof(Z80AsmTokenType.Number), 
                typeService.GetClassificationType("Z80Number"));
            _z80AsmTypes.Add(nameof(Z80AsmTokenType.Identifier), 
                typeService.GetClassificationType("Z80Identifier"));
            _z80AsmTypes.Add(nameof(Z80AsmTokenType.Breakpoint),
                typeService.GetClassificationType("Z80Breakpoint"));
            _z80AsmTypes.Add(nameof(Z80AsmTokenType.CurrentBreakpoint),
                typeService.GetClassificationType("Z80CurrentBreakpoint"));
        }

        /// <summary>
        /// Gets all the tags that intersect the specified spans. 
        /// </summary>
        /// <param name="spans">The spans to visit.</param>
        /// <returns>
        /// A <see cref="T:Microsoft.VisualStudio.Text.Tagging.TagSpan`1" /> for each tag.
        /// </returns>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var tagSpan in _aggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return
                    new TagSpan<ClassificationTag>(tagSpans[0],
                        new ClassificationTag(_z80AsmTypes[tagSpan.Tag.Type]));
            }
        }

        /// <summary>
        /// Occurs when tags are added to or removed from the provider.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }

#pragma warning restore 67
#pragma warning restore 649
}