using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Disassembler;

// ReSharper disable ExplicitCallerInfoArgument

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    public class DisassemblyViewModel: SpectrumGenericToolWindowViewModel
    {
        private DisassembyAnnotations _disassembyAnnotations;
        private ObservableCollection<DisassemblyItemViewModel> _disassemblyItems;


        /// <summary>
        /// The disassembly items belonging to this project
        /// </summary>
        public ObservableCollection<DisassemblyItemViewModel> DisassemblyItems
        {
            get => _disassemblyItems;
            private set => Set(ref _disassemblyItems, value);
        }

        /// <summary>
        /// The disassembly project belonging to this view model
        /// </summary>
        public DisassembyAnnotations DisassembyAnnotations
        {
            get => _disassembyAnnotations;
            set => Set(ref _disassembyAnnotations, value);
        }

        /// <summary>
        /// Gets the line index
        /// </summary>
        public Dictionary<ushort, int> LineIndexes { get; private set; }

        /// <summary>
        /// Toggles the breakpoint associated with the selected item
        /// </summary>
        public RelayCommand ToggleBreakpointCommand { get; set; }

        /// <summary>
        /// Selected disassembly items
        /// </summary>
        public IList<DisassemblyItemViewModel> SelectedItems => DisassemblyItems.Where(item => item.IsSelected).ToList();

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public DisassemblyViewModel()
        {
            DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>();
            LineIndexes = new Dictionary<ushort, int>();
            ToggleBreakpointCommand = new RelayCommand(OnToggleBreakpoint);

            if (IsInDesignMode)
            {
                DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>
                {
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel()
                };
            }
        }

        /// <summary>
        /// Disassembles the current virtual machine's memory
        /// </summary>
        public void Disassemble()
        {
            if (SpectrumVmViewModel.SpectrumVm == null) return;
            var map = new MemoryMap
            {
                new MemorySection(0x0000, 0x3CFF),
                new MemorySection(0x3D00, 0x3FFF, MemorySectionType.ByteArray),
                new MemorySection(0x4000, 0x5AFF, MemorySectionType.Skip),
                new MemorySection(0x5B00, 0x5BFF, MemorySectionType.Skip),
                new MemorySection(0x5C00, 0x5CB5, MemorySectionType.WordArray),
                new MemorySection(0x5CB6, 0xFFFF)
            };

            if (VmStopped) return;

            var project = new DisassembyAnnotations(map);
            var disassembler = new Z80Disassembler(project, SpectrumVmViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer());
            var output = disassembler.Disassemble();
            DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>();
            LineIndexes = new Dictionary<ushort, int>();

            var idx = 0;
            foreach (var item in output.OutputItems)
            {
                DisassemblyItems.Add(new DisassemblyItemViewModel(SpectrumVmViewModel, item));
                LineIndexes.Add(item.Address, idx++);
            }
        }

        /// <summary>
        /// Clears the disassembled items
        /// </summary>
        public void Clear()
        {
            DisassemblyItems.Clear();
        }

        /// <summary>
        /// Handle to Toggle Breakpoints command
        /// </summary>
        private void OnToggleBreakpoint()
        {
            var items = SelectedItems;
            foreach (var selItem in items)
            {
                var address = selItem.Item.Address;
                var breakpoints = SpectrumVmViewModel.DebugInfoProvider.Breakpoints;
                if (breakpoints.Contains(address))
                {
                    breakpoints.Remove(address);
                }
                else
                {
                    breakpoints.Add(address);
                }
                selItem.RaisePropertyChanged(nameof(selItem.HasBreakpoint));
            }
        }
    }
}