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
        private Dictionary<int, DisassemblyAnnotation> _annotations;
        private DisassemblyAnnotation _selectedBank;
        private int _selectedBankIndex;

        /// <summary>
        /// The DisassemblyAnnotations of the .disann file
        /// </summary>
        public Dictionary<int, DisassemblyAnnotation> Annotations
        {
            get => _annotations;
            set
            {
                if (!Set(ref _annotations, value)) return;

                RaisePropertyChanged(nameof(BanksCount));
                RaisePropertyChanged(nameof(ShowBanks));
                RaisePropertyChanged(nameof(BankIndexes));
            }
        }

        /// <summary>
        /// Number of banks
        /// </summary>
        public int BanksCount => _annotations.Count;

        /// <summary>
        /// Indicates if banks should be displayed
        /// </summary>
        public bool ShowBanks => BanksCount > 1;

        /// <summary>
        /// The bank indexses to display
        /// </summary>
        public IEnumerable<int> BankIndexes => _annotations.Keys.OrderBy(k => k);

        /// <summary>
        /// The selected annotation
        /// </summary>
        public DisassemblyAnnotation SelectedBank
        {
            get => _selectedBank;
            set => Set(ref _selectedBank, value);
        }

        /// <summary>
        /// The selected bank index
        /// </summary>
        public int SelectedBankIndex
        {
            get => _selectedBankIndex;
            set => Set(ref _selectedBankIndex, value);
        }

        /// <summary>
        /// Disassembly flag
        /// </summary>
        public SpectrumSpecificDisassemblyFlags DisassemblyFlags => _selectedBank.DisassemblyFlags;

        /// <summary>
        /// The list of labels ordered by address
        /// </summary>
        public List<KeyValuePair<ushort, string>> LabelsOrdered 
            => _selectedBank.Labels.OrderBy(l => l.Key).ToList();

        /// <summary>
        /// The list of literals ordered by address
        /// </summary>
        public List<KeyValuePair<ushort, List<string>>> LiteralsOrdered
            => _selectedBank.Literals.OrderBy(l => l.Key).ToList();

        /// <summary>
        /// The list of comments ordered by address
        /// </summary>
        public List<KeyValuePair<ushort, string>> CommentsOrdered
            => _selectedBank.Comments.OrderBy(l => l.Key).ToList();

        /// <summary>
        /// The list of comments ordered by address
        /// </summary>
        public List<KeyValuePair<ushort, string>> PrefixCommentsOrdered
            => _selectedBank.PrefixComments.OrderBy(l => l.Key).ToList();

        /// <summary>
        /// The list of replacements ordered
        /// </summary>
        public List<KeyValuePair<string, string>> ReplacementsOrdered
            => _selectedBank.LiteralReplacements.GroupBy(r => r.Value,
                r => r.Key,
                (key, g) => new KeyValuePair<string, string>(key, string.Join(", ", g.Select(i => $"{i:X4}"))))
                .OrderBy(i => i.Key)
                .ToList();
    }
}