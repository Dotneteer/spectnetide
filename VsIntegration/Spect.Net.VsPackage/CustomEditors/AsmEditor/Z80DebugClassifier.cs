using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// Classifier for the Z80 debugger
    /// </summary>
    public class Z80DebugClassifier : ITagger<ClassificationTag>
    {
        private readonly ITagAggregator<Z80DebugTokenTag> _aggregator;
        private readonly IClassificationType _breakpoint;
        private readonly IClassificationType _currentBreakpoint;

        public Z80DebugClassifier(ITagAggregator<Z80DebugTokenTag> aggregator,
            IClassificationTypeRegistryService typeService)
        {
            _aggregator = aggregator;
            _breakpoint = typeService.GetClassificationType("Z80Breakpoint");
            _currentBreakpoint = typeService.GetClassificationType("Z80CurrentBreakpoint");
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
                var classification = tagSpan.Tag.Type == "Z80Breakpoint"
                    ? _breakpoint
                    : _currentBreakpoint;
                yield return
                    new TagSpan<ClassificationTag>(tagSpans[0],
                        new ClassificationTag(classification));
            }
        }

        /// <summary>
        /// Occurs when tags are added to or removed from the provider.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}