using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

#pragma warning disable 67

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// Classifier for the Z80 assembly language
    /// </summary>
    public class Z80AsmClassifier : ITagger<ClassificationTag>
    {
        private readonly ITagAggregator<Z80AsmTokenTag> _aggregator;
        private readonly Dictionary<Z80AsmTokenType, IClassificationType> _z80AsmTypes =
            new Dictionary<Z80AsmTokenType, IClassificationType>();

        public Z80AsmClassifier(ITagAggregator<Z80AsmTokenTag> aggregator,
            IClassificationTypeRegistryService typeService)
        {
            _aggregator = aggregator;
            _z80AsmTypes.Add(Z80AsmTokenType.Label, typeService.GetClassificationType("Z80Label"));
            _z80AsmTypes.Add(Z80AsmTokenType.Pragma, typeService.GetClassificationType("Z80Pragma"));
            _z80AsmTypes.Add(Z80AsmTokenType.Directive, typeService.GetClassificationType("Z80Directive"));
            _z80AsmTypes.Add(Z80AsmTokenType.Include, typeService.GetClassificationType("Z80IncludeDirective"));
            _z80AsmTypes.Add(Z80AsmTokenType.Instruction, typeService.GetClassificationType("Z80Instruction"));
            _z80AsmTypes.Add(Z80AsmTokenType.Comment, typeService.GetClassificationType("Z80Comment"));
            _z80AsmTypes.Add(Z80AsmTokenType.Number, typeService.GetClassificationType("Z80Number"));
            _z80AsmTypes.Add(Z80AsmTokenType.Identifier, typeService.GetClassificationType("Z80Identifier"));
            _z80AsmTypes.Add(Z80AsmTokenType.String, typeService.GetClassificationType("Z80String"));
            _z80AsmTypes.Add(Z80AsmTokenType.Function, typeService.GetClassificationType("Z80Function"));
            _z80AsmTypes.Add(Z80AsmTokenType.Breakpoint, typeService.GetClassificationType("Z80Breakpoint"));
            _z80AsmTypes.Add(Z80AsmTokenType.CurrentBreakpoint, typeService.GetClassificationType("Z80CurrentBreakpoint"));
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
}

#pragma warning restore 67
