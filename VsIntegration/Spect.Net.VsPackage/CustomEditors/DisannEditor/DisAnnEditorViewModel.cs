using System.Collections.Generic;
using System.Linq;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.CustomEditors.DisannEditor
{
    /// <summary>
    /// This class represents the view model of the DisAnnEditor
    /// </summary>
    public class DisAnnEditorViewModel : EnhancedViewModelBase
    {
        private DisassemblyAnnotation _annotations;

        /// <summary>
        /// The DisassemblyAnnotations of the .disann file
        /// </summary>
        public DisassemblyAnnotation Annotations
        {
            get => _annotations;
            set => Set(ref _annotations, value);
        }

        /// <summary>
        /// The list of labels ordered by address
        /// </summary>
        public List<KeyValuePair<ushort, string>> LabelsOrdered 
            => _annotations.Labels.OrderBy(l => l.Key).ToList();

        /// <summary>
        /// The list of literals ordered by address
        /// </summary>
        public List<KeyValuePair<ushort, List<string>>> LiteralsOrdered
            => _annotations.Literals.OrderBy(l => l.Key).ToList();

        /// <summary>
        /// The list of comments ordered by address
        /// </summary>
        public List<KeyValuePair<ushort, string>> CommentsOrdered
            => _annotations.Comments.OrderBy(l => l.Key).ToList();

        /// <summary>
        /// The list of comments ordered by address
        /// </summary>
        public List<KeyValuePair<ushort, string>> PrefixCommentsOrdered
            => _annotations.PrefixComments.OrderBy(l => l.Key).ToList();

        /// <summary>
        /// The list of replacements ordered
        /// </summary>
        public List<KeyValuePair<string, string>> ReplacementsOrdered
            => _annotations.LiteralReplacements.GroupBy(r => r.Value,
                r => r.Key,
                (key, g) => new KeyValuePair<string, string>(key, string.Join(", ", g.Select(i => $"{i:X4}"))))
                .OrderBy(i => i.Key)
                .ToList();
    }
}