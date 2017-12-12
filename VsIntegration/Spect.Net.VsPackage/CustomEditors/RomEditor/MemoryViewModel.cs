using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.VsPackage.ToolWindows.Memory;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// This view model represents the spectrum memory
    /// </summary>
    public class MemoryViewModel: EnhancedViewModelBase, IDisassemblyItemParent
    {
        private byte[] _memoryBuffer;
        private bool _showPrompt;
        private bool _allowDisassembly;
        private bool _showDisassembly;
        private ObservableCollection<DisassemblyItemViewModel> _disassemblyItems;

        /// <summary>
        /// The contents of the memory
        /// </summary>
        public byte[] MemoryBuffer
        {
            get => _memoryBuffer;
            set
            {
                if (!Set(ref _memoryBuffer, value)) return;
                InitMemoryLines();
            }
        }

        /// <summary>
        /// Signs if command prompt is displayed
        /// </summary>
        public bool ShowPrompt
        {
            get => _showPrompt;
            set => Set(ref _showPrompt, value);
        }

        /// <summary>
        /// Signs if disassembly commands are allowed
        /// </summary>
        public bool AllowDisassembly
        {
            get => _allowDisassembly;
            set => Set(ref _allowDisassembly, value);
        }

        /// <summary>
        /// Signs if the disassembly panel should be displayed
        /// </summary>
        public bool ShowDisassembly
        {
            get => _showDisassembly;
            set => Set(ref _showDisassembly, value);
        }

        /// <summary>
        /// Gets the line index
        /// </summary>
        public Dictionary<ushort, int> LineIndexes { get; private set; }

        /// <summary>
        /// Creates the initial state of the view model
        /// </summary>
        public MemoryViewModel()
        {
            ShowPrompt = true;
            if (!IsInDesignMode) return;

            MemoryBuffer = new byte[]
            {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
            };

            DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>
            {
                new DisassemblyItemViewModel(),
                new DisassemblyItemViewModel(),
                new DisassemblyItemViewModel(),
                new DisassemblyItemViewModel()
            };
        }

        /// <summary>
        /// The lines (16 byte each) that represent the entire memory
        /// </summary>
        public ObservableCollection<MemoryLineViewModel> MemoryLines { get; } =
            new ObservableCollection<MemoryLineViewModel>();

        /// <summary>
        /// The disassembly items belonging to this ROM editor
        /// </summary>
        public ObservableCollection<DisassemblyItemViewModel> DisassemblyItems
        {
            get => _disassemblyItems;
            set => Set(ref _disassemblyItems, value);
        }

        /// <summary>
        /// Creates disassembly from the specified address
        /// </summary>
        /// <param name="startAddress"></param>
        public void Disassembly(ushort startAddress)
        {
            var map = new MemoryMap {new MemorySection(startAddress, (ushort) (MemoryBuffer.Length - 1))};
            var disassembler = new Z80Disassembler(map, MemoryBuffer);
            var output = disassembler.Disassemble();
            DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>();
            LineIndexes = new Dictionary<ushort, int>();

            var idx = 0;
            foreach (var item in output.OutputItems)
            {
                DisassemblyItems.Add(new DisassemblyItemViewModel(this, item));
                LineIndexes.Add(item.Address, idx++);
            }
        }

        /// <summary>
        /// Initializes the memory lines with empty values
        /// </summary>
        private void InitMemoryLines()
        {
            MemoryLines.Clear();
            if (_memoryBuffer == null) return;

            for (var addr = 0x0000; addr < _memoryBuffer.Length; addr += 16)
            {
                var line = new MemoryLineViewModel(addr, _memoryBuffer.Length - 1);
                line.BindTo(MemoryBuffer);
                MemoryLines.Add(line);
            }
        }

        /// <summary>
        /// Gets the label for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="label">Label, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        public bool GetLabel(ushort address, out string label)
        {
            label = null;
            return false;
        }

        /// <summary>
        /// Gets the comment for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="comment">Comment, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        public bool GetComment(ushort address, out string comment)
        {
            comment = null;
            return false;
        }

        /// <summary>
        /// Gets the prefix comment for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="comment">Prefix comment, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        public bool GetPrefixComment(ushort address, out string comment)
        {
            comment = null;
            return false;
        }

        /// <summary>
        /// Gets the literal replacement for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="symbol">Symbol, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        public bool GetLiteralReplacement(ushort address, out string symbol)
        {
            symbol = null;
            return false;
        }

        /// <summary>
        /// Checks if the specified address has a breakpoint
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address has a breakpoint; otherwise, false
        /// </returns>
        public bool HasBreakpoint(ushort address)
        {
            return false;
        }

        /// <summary>
        /// Checks if the specified address is the current instruction
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address is the current instruction; otherwise, false
        /// </returns>
        public bool IsCurrentInstruction(ushort address)
        {
            return false;
        }
    }
}