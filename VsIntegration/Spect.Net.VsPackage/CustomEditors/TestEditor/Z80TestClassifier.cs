using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

#pragma warning disable 67

namespace Spect.Net.VsPackage.CustomEditors.TestEditor
{
    /// <summary>
    /// Classifier for the Z80 test language
    /// </summary>
    public class Z80TestClassifier : ITagger<ClassificationTag>
    {
        private readonly ITagAggregator<Z80TestTokenTag> _aggregator;
        private readonly Dictionary<Z80TestTokenType, IClassificationType> _z80TestTypes =
            new Dictionary<Z80TestTokenType, IClassificationType>();

        public Z80TestClassifier(ITagAggregator<Z80TestTokenTag> aggregator,
            IClassificationTypeRegistryService typeService)
        {
            _aggregator = aggregator;
            _z80TestTypes.Add(Z80TestTokenType.Keyword, typeService.GetClassificationType("Z80TestKeyword"));
            _z80TestTypes.Add(Z80TestTokenType.Comment, typeService.GetClassificationType("Z80TestComment"));
            _z80TestTypes.Add(Z80TestTokenType.Number, typeService.GetClassificationType("Z80TestNumber"));
            _z80TestTypes.Add(Z80TestTokenType.Identifier, typeService.GetClassificationType("Z80TestIdentifier"));
            _z80TestTypes.Add(Z80TestTokenType.Z80Key, typeService.GetClassificationType("Z80TestKey"));
            _z80TestTypes.Add(Z80TestTokenType.Breakpoint, typeService.GetClassificationType("Z80TestBreakpoint"));
            _z80TestTypes.Add(Z80TestTokenType.CurrentBreakpoint, typeService.GetClassificationType("Z80TestCurrentBreakpoint"));
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
                        new ClassificationTag(_z80TestTypes[tagSpan.Tag.Type]));
            }
        }

        /// <summary>
        /// Occurs when tags are added to or removed from the provider.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}

#pragma warning restore 67
