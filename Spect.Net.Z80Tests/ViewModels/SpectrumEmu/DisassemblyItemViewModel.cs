using System.Linq;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.Z80Tests.Mvvm.Navigation;

namespace Spect.Net.Z80Tests.ViewModels.SpectrumEmu
{
    /// <summary>
    /// This class represents the view model of a single disassembly item
    /// </summary>
    public class DisassemblyItemViewModel: ViewModelBaseWithDesignTimeFix
    {
        private DisassemblyItem _item;
        private bool _isSelected;

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
        /// The debug view model relatesd to this item
        /// </summary>
        public SpectrumDebugViewModel DebugViewModel { get; }

        /// <summary>
        /// Default constructor for support desing time behavior
        /// </summary>
        public DisassemblyItemViewModel()
        {
        }

        /// <summary>
        /// Initializes the class with the specified disassembly item
        /// </summary>
        /// <param name="item">Item to use in this view model</param>
        /// <param name="debugViewModel">The debug view model</param>
        public DisassemblyItemViewModel(DisassemblyItem item, SpectrumDebugViewModel debugViewModel)
        {
            _item = item;
            DebugViewModel = debugViewModel;
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
            DebugViewModel.DebugInfoProvider?.Breakpoints?.Contains(Item.Address) ?? false;

        /// <summary>
        /// Indicates if this item has prefix comments
        /// </summary>
        public bool HasPrefixComment => Item.PrefixComment != null;

        /// <summary>
        /// Indicates if this item is the current instruction pointed by
        /// the Z80 CPU's PC register
        /// </summary>
        public bool IsCurrentInstruction => DebugViewModel != null 
            && DebugViewModel.VmState == VmState.Paused
            && DebugViewModel.SpectrumVm?.Cpu.Registers.PC == Item.Address;
    }
}