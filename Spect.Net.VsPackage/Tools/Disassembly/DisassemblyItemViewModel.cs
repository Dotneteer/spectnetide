using System.Linq;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// This class represents the view model of a single disassembly item
    /// </summary>
    public class DisassemblyItemViewModel: EnhancedViewModelBase
    {
        private DisassemblyItem _item;
        private bool _isSelected;

        public SpectrumVmViewModel SpectrumVmViewModel { get; }
        /// <summary>
        /// Ths raw disassembly item
        /// </summary>
        public DisassemblyItem Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        /// <summary>
        /// Indicates if this item is selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        /// <summary>
        /// Default constructor for support desing time behavior
        /// </summary>
        public DisassemblyItemViewModel()
        {
            if (!IsInDesignMode) return;

            _item = new DisassemblyItem(0x1234)
            {
                OpCodes = {0x01, 0x02, 0x03, 0x04},
                Label = "L1234",
                Instruction = "ld a,(ix+03H)",
                Comment = "This is a long comment..."
            };
        }

        /// <summary>
        /// Initializes the class with the specified disassembly item
        /// </summary>
        /// <param name="spectrumVm">The Spectrum virtual machine view model</param>
        /// <param name="item">Item to use in this view model</param>
        public DisassemblyItemViewModel(SpectrumVmViewModel spectrumVm, DisassemblyItem item)
        {
            SpectrumVmViewModel = spectrumVm;
            _item = item;
        }

        /// <summary>
        /// Address in hex format
        /// </summary>
        public string AddressFormatted => $"{Item.Address:X4}";

        /// <summary>
        /// Operation codex in hex format
        /// </summary>
        public string OpCodesFormatted => 
            string.Join(" ", Item.OpCodes.Select(op => $"{op:X2}")).PadRight(12);

        /// <summary>
        /// Label formatted for output
        /// </summary>
        public string LabelFormatted => 
            Item.Label == null ? string.Empty : Item.Label + ":";

        /// <summary>
        /// Comment formatted for output
        /// </summary>
        public string CommentFormatted => 
            Item.Comment == null ? string.Empty : "; " + Item.Comment;

        /// <summary>
        /// Comment formatted for output
        /// </summary>
        public string PrefixCommentFormatted => 
            Item.PrefixComment == null ? string.Empty : "; " + Item.PrefixComment;

        /// <summary>
        /// Indicates if there is a breakpoint on this item
        /// </summary>
        public bool HasBreakpoint => 
            SpectrumVmViewModel.DebugInfoProvider?.Breakpoints?.Contains(Item.Address) ?? true;

        /// <summary>
        /// Indicates if this item has prefix comments
        /// </summary>
        public bool HasPrefixComment => Item.PrefixComment != null;

        /// <summary>
        /// Indicates if this item is the current instruction pointed by
        /// the Z80 CPU's PC register
        /// </summary>
        public bool IsCurrentInstruction => SpectrumVmViewModel != null 
            && SpectrumVmViewModel.VmState == SpectrumVmState.Paused
            && SpectrumVmViewModel.SpectrumVm?.Cpu.Registers.PC == Item.Address;
    }
}