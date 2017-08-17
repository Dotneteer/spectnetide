using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.VsPackage.Vsx;
// ReSharper disable AssignNullToNotNullAttribute

// ReSharper disable ExplicitCallerInfoArgument

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    public class DisassemblyViewModel: SpectrumGenericToolWindowViewModel
    {
        private ObservableCollection<DisassemblyItemViewModel> _disassemblyItems;
        private bool _saveRomChangesToRom;

        /// <summary>
        /// The disassembly items belonging to this project
        /// </summary>
        public ObservableCollection<DisassemblyItemViewModel> DisassemblyItems
        {
            get => _disassemblyItems;
            private set => Set(ref _disassemblyItems, value);
        }

        /// <summary>
        /// Gets the line index
        /// </summary>
        public Dictionary<ushort, int> LineIndexes { get; private set; }

        /// <summary>
        /// Stores the object that handles annotations and their persistence
        /// </summary>
        public DisassemblyAnnotationHandler AnnotationHandler { get; private set; }

        /// <summary>
        /// Signs that ROM changes should be saved to the ROM annotations file
        /// </summary>
        public bool SaveRomChangesToRom
        {
            get => _saveRomChangesToRom;
            set => Set(ref _saveRomChangesToRom, value);
        }

        /// <summary>
        /// Toggles the breakpoint associated with the selected item
        /// </summary>
        public RelayCommand ToggleBreakpointCommand { get; set; }

        /// <summary>
        /// Selected disassembly items
        /// </summary>
        public IList<DisassemblyItemViewModel> SelectedItems => DisassemblyItems.Where(item => item.IsSelected).ToList();

        /// <summary>
        /// The annotations that should be handled by the disassembler
        /// </summary>
        public DisassemblyAnnotation Annotations => AnnotationHandler.MergedAnnotations;

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public DisassemblyViewModel()
        {
            if (IsInDesignMode)
            {
                DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>
                {
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel()
                };
                return;
            }

            InitDisassembly();
            ToggleBreakpointCommand = new RelayCommand(OnToggleBreakpoint);
        }

        /// <summary>
        /// Obtain the machine view model from the solution
        /// </summary>
        protected override void OnSolutionOpened(SolutionOpenedMessage msg)
        {
            base.OnSolutionOpened(msg);
            InitDisassembly();
        }

        /// <summary>
        /// Disassembles the current virtual machine's memory
        /// </summary>
        public void Disassemble()
        {
            if (MachineViewModel.SpectrumVm == null) return;
            if (VmStopped) return;

            var disassembler = CreateDisassembler();
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
        /// Creates a disassembler for the curent machine
        /// </summary>
        /// <returns></returns>
        private Z80Disassembler CreateDisassembler()
        {
            var disassembler = new Z80Disassembler(AnnotationHandler.MergedAnnotations.MemoryMap, 
                MachineViewModel.SpectrumVm.MemoryDevice.GetMemoryBuffer());
            return disassembler;
        }

        /// <summary>
        /// Initializes disassembly items and annotations
        /// </summary>
        private void InitDisassembly()
        {
            DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>();
            LineIndexes = new Dictionary<ushort, int>();
            var workspace = VsxPackage.GetPackage<SpectNetPackage>().CurrentWorkspace;
            string romAnnotationFile = null;
            var romItem = workspace.RomItem;
            if (romItem != null)
            {
                romAnnotationFile =
                    Path.Combine(
                        Path.GetDirectoryName(romItem.Filename),
                        Path.GetFileNameWithoutExtension(romItem.Filename)) + ".disann";
            }

            var customAnnotationFile = workspace.AnnotationItem?.Filename;
            AnnotationHandler = new DisassemblyAnnotationHandler(this, romAnnotationFile, customAnnotationFile);
            AnnotationHandler.RestoreAnnotations();
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
                var breakpoints = MachineViewModel.DebugInfoProvider.Breakpoints;
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