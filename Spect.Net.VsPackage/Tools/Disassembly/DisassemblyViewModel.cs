using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.Wpf.Providers;

// ReSharper disable ExplicitCallerInfoArgument

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    public class DisassemblyViewModel: SpectrumGenericToolWindowViewModel
    {
        private IReadOnlyList<DisassemblyItemViewModel> _disassemblyItems;
        private DisassembyAnnotations _disassembyAnnotations;

        /// <summary>
        /// The disassembly items belonging to this project
        /// </summary>
        public IReadOnlyList<DisassemblyItemViewModel> DisassemblyItems
        {
            get => _disassemblyItems;
            set => Set(ref _disassemblyItems, value);
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
        /// Toggles the breakpoint associated with the selected item
        /// </summary>
        public RelayCommand ToggleBreakpointCommand { get; set; }

        /// <summary>
        /// Selected disassembly items
        /// </summary>
        public IList<DisassemblyItemViewModel> SelectedItems => _disassemblyItems.Where(item => item.IsSelected).ToList();

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public DisassemblyViewModel()
        {
            DisassemblyItems = new List<DisassemblyItemViewModel>();
            ToggleBreakpointCommand = new RelayCommand(OnToggleBreakpoint);

            if (IsInDesignMode)
            {
                DisassemblyItems = new List<DisassemblyItemViewModel>
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
            var osInfo = new ResourceRomProvider().LoadRom("ZXSpectrum48");
            var map = new MemoryMap
            {
                new MemorySection(0x0000, 0x3CFF),
                new MemorySection(0x3D00, 0x3FFF, MemorySectionType.ByteArray),
                new MemorySection(0x4000, 0x5AFF, MemorySectionType.Skip),
                new MemorySection(0x5B00, 0x5B0D, MemorySectionType.WordArray),
                new MemorySection(0x5B0E, 0x5B4C, MemorySectionType.WordArray)
            };
            var memory = new byte[0x10000];
            osInfo.RomBytes.CopyTo(memory, 0);
            var project = new DisassembyAnnotations(map);
            var disassembler = new Z80Disassembler(project, memory);
            var output = disassembler.Disassemble();
            DisassemblyItems = output.OutputItems
                .Select(di => new DisassemblyItemViewModel(di))
                .ToList();
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