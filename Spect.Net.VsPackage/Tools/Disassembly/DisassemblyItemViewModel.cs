using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Mvvm;

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// This class represents the view model of a single disassembly item
    /// </summary>
    public class DisassemblyItemViewModel: EnhancedViewModelBase
    {
        private DisassemblyItem _item;
        private bool _isSelected;

        /// <summary>
        /// The parent view model that holds the annotations
        /// </summary>
        public IDisassemblyItemParent Parent { get; }

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
        /// <param name="parent">The parent view model</param>
        /// <param name="item">Item to use in this view model</param>
        public DisassemblyItemViewModel(IDisassemblyItemParent parent, DisassemblyItem item)
        {
            Parent = parent;
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
            Parent.Annotations.Labels.TryGetValue(Item.Address, out string label)
                ? $"{label}:"
                : (Item.HasLabel ? $"L{Item.Address:X4}:" : "");

        /// <summary>
        /// Instruction formatted for output;
        /// </summary>
        public string InstructionFormatted
        {
            get
            {
                if (Item.TokenLength == 0)
                {
                    return Item.Instruction;
                }

                string symbol;
                if (Item.HasLabelSymbol)
                {
                    if (!Parent.Annotations.Labels.TryGetValue(Item.SymbolValue, out string label))
                    {
                        return Item.Instruction;
                    }
                    symbol = label;
                }
                else if (!Parent.Annotations.LiteralReplacements.TryGetValue(Item.Address, out symbol))
                {
                    return Item.Instruction;
                }

                return Item.Instruction.Substring(0, Item.TokenPosition) + symbol +
                       Item.Instruction.Substring(Item.TokenPosition + Item.TokenLength);
            }
        }

        /// <summary>
        /// Comment formatted for output
        /// </summary>
        public string CommentFormatted => 
            Parent.Annotations.Comments.TryGetValue(Item.Address, out string comment)
                ? comment : string.Empty;

        /// <summary>
        /// Comment formatted for output
        /// </summary>
        public string PrefixCommentFormatted =>
            Parent.Annotations.PrefixComments.TryGetValue(Item.Address, out string comment)
                ? comment : string.Empty;


        /// <summary>
        /// Indicates if there is a breakpoint on this item
        /// </summary>
        public bool HasBreakpoint => 
            Parent?.MachineViewModel?.DebugInfoProvider?.Breakpoints?.Contains(Item.Address) ?? false;

        /// <summary>
        /// Indicates if this item has prefix comments
        /// </summary>
        public bool HasPrefixComment => Parent.Annotations.PrefixComments.ContainsKey(Item.Address);

        /// <summary>
        /// Indicates if this item is the current instruction pointed by
        /// the Z80 CPU's PC register
        /// </summary>
        public bool IsCurrentInstruction => Parent.MachineViewModel != null 
            && Parent.MachineViewModel.VmState == VmState.Paused
            && Parent.MachineViewModel.SpectrumVm?.Cpu.Registers.PC == Item.Address;
    }
}