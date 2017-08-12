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
                OpCodes = "01 02 03 04",
                HasLabel = true,
                Instruction = "ld a,(ix+03H)"
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
        public string OpCodesFormatted => Item.OpCodes;

        /// <summary>
        /// Label formatted for output
        /// </summary>
        public string LabelFormatted =>
            Item.HasLabel ? $"L{Item.Address:X4}:" : "";

        /// <summary>
        /// Comment formatted for output
        /// </summary>
        public string CommentFormatted => string.Empty; // TODO: apply comment from annotation file

        /// <summary>
        /// Comment formatted for output
        /// </summary>
        public string PrefixCommentFormatted => string.Empty; // TODO: apply comment from annotation file

        /// <summary>
        /// Indicates if there is a breakpoint on this item
        /// </summary>
        public bool HasBreakpoint => 
            SpectrumVmViewModel.DebugInfoProvider?.Breakpoints?.Contains(Item.Address) ?? true;

        /// <summary>
        /// Indicates if this item has prefix comments
        /// </summary>
        public bool HasPrefixComment => false; // TODO: apply comment from annotation file

        /// <summary>
        /// Indicates if this item is the current instruction pointed by
        /// the Z80 CPU's PC register
        /// </summary>
        public bool IsCurrentInstruction => SpectrumVmViewModel != null 
            && SpectrumVmViewModel.VmState == SpectrumVmState.Paused
            && SpectrumVmViewModel.SpectrumVm?.Cpu.Registers.PC == Item.Address;
    }
}