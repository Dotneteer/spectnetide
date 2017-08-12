using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.VsPackage.CodeDiscovery;
using Spect.Net.VsPackage.Vsx;

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
            if (VmStopped) return;

            var disassembler = CreateDisassembler();
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
        /// Refrehses the disassembly line with the given address
        /// </summary>
        /// <param name="addr"></param>
        public void RefreshDisassembly(ushort addr)
        {
            // --- Get the item, provided it exists
            if (!LineIndexes.TryGetValue(addr, out int index))
            {
                return;
            }

            var startAddr = DisassemblyItems[index].Item.Address;
            var disassembler = CreateDisassembler();
            var output = disassembler.Disassemble(startAddr, (ushort)(startAddr + 1));
            if (output.OutputItems.Count > 0)
            {
                DisassemblyItems[index].Item = output.OutputItems[0];
            }
        }

        /// <summary>
        /// Handled the LABEL command
        /// </summary>
        /// <param name="addr">Label address</param>
        /// <param name="label">Label text</param>
        public void HandleLabelCommand(ushort addr, string label)
        {
            _annotationHandler.AddCustomLabel(addr, label);
            RefreshDisassembly(addr);
        }

        /// <summary>
        /// Accesses the annotation handler of the package
        /// </summary>
        private AnnotationHandler _annotationHandler => VsxPackage.GetPackage<SpectNetPackage>().AnnotationHandler;

        /// <summary>
        /// Creates a disassembler for the curent machine
        /// </summary>
        /// <returns></returns>
        private Z80Disassembler CreateDisassembler()
        {
            // TODO: Use the memory map of the project
            var map = new MemoryMap
            {
                new MemorySection(0x0000, 0x3CFF),
                new MemorySection(0x3D00, 0x3FFF, MemorySectionType.ByteArray),
                new MemorySection(0x4000, 0x5AFF, MemorySectionType.Skip),
                new MemorySection(0x5B00, 0x5BFF, MemorySectionType.Skip),
                new MemorySection(0x5C00, 0x5CB5, MemorySectionType.WordArray),
                new MemorySection(0x5CB6, 0xFFFF)
            };


            var project = new DisassembyAnnotations(map);
            var disassembler = new Z80Disassembler(project, SpectrumVmViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer());
            return disassembler;
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