using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// This view model represents the ZX Spectrum memory
    /// </summary>
    public class MemoryToolWindowViewModel : SpectrumGenericToolWindowViewModel
    {
        private bool _fullViewMode;
        private bool _romViewMode;
        private bool _ramBankViewMode;
        private int _romIndex;
        private int _ramBankIndex;

        public ObservableCollection<MemoryLineViewModel> MemoryLines { get; } =
            new ObservableCollection<MemoryLineViewModel>();

        /// <summary>
        /// The full 64K memory is displayed
        /// </summary>
        public bool FullViewMode
        {
            get => _fullViewMode;
            set => Set(ref _fullViewMode, value);
        }

        /// <summary>
        /// A ROM page is displayed
        /// </summary>
        public bool RomViewMode
        {
            get => _romViewMode;
            set => Set(ref _romViewMode, value);
        }

        /// <summary>
        /// A RAM bank page is displayed
        /// </summary>
        public bool RamBankViewMode
        {
            get => _ramBankViewMode;
            set => Set(ref _ramBankViewMode, value);
        }

        /// <summary>
        /// The index of the ROM page displayed in ROM view mode
        /// </summary>
        public int RomIndex
        {
            get => _romIndex;
            set => Set(ref _romIndex, value);
        }

        /// <summary>
        /// The index of the RAM page displayed in RAM bank view mode
        /// </summary>
        public int RamBankIndex
        {
            get => _ramBankIndex;
            set => Set(ref _ramBankIndex, value);
        }

        /// <summary>
        /// This flag indicates if the Bank view is allowed or not
        /// </summary>
        public bool BankViewAllowed => (MachineViewModel?.SpectrumVm
                                            ?.RomConfiguration?.NumberOfRoms ?? 0) > 1;

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public MemoryToolWindowViewModel()
        {
            if (IsInDesignMode) return;

            EvaluateState();
            if (VmNotStopped)
            {
                InitMemoryLines();
                RefreshMemoryLines();
            }
        }

        /// <summary>
        /// Sets the current view to full view mode
        /// </summary>
        public void SetFullView()
        {
            FullViewMode = true;
            RomViewMode = false;
            RamBankViewMode = false;
            InitMemoryLines();
        }

        /// <summary>
        /// Sets the current view to ROM view mode
        /// </summary>
        /// <param name="romIndex">The ROM page to show</param>
        public void SetRomView(int romIndex)
        {
            FullViewMode = false;
            RomViewMode = false;
            RamBankViewMode = false;
            RomIndex = romIndex;
            RomViewMode = true;
            InitMemoryLines();
        }

        /// <summary>
        /// Sets the current view to RAM bank view mode
        /// </summary>
        /// <param name="ramBankIndex">The RAM bank to show</param>
        public void SetRamBankView(int ramBankIndex)
        {
            FullViewMode = false;
            RomViewMode = false;
            RamBankViewMode = false;
            RamBankIndex = ramBankIndex;
            RamBankViewMode = true;
            InitMemoryLines();
        }

        /// <summary>
        /// Refreshes the specified memory line
        /// </summary>
        /// <param name="addr">Address of the memory line</param>
        public void RefreshMemoryLine(int addr)
        {
            var memory = GetMemoryBuffer();
            var length = GetMemoryLength();
            if (memory == null || length == null) return;

            if (addr < 0 || addr >= length) return;

            var memLine = new MemoryLineViewModel(addr);
            memLine.BindTo(memory);
            var lineNo = addr >> 4;
            if (lineNo < MemoryLines.Count)
            {
                MemoryLines[lineNo] = memLine;
            }
        }

        /// <summary>
        /// Refreshes all memory lines
        /// </summary>
        public void RefreshMemoryLines()
        {
            var memory = GetMemoryBuffer();
            var length = GetMemoryLength();
            if (memory == null || length == null) return;

            for (var addr = 0x0000; addr < length; addr += 16)
            {
                RefreshMemoryLine((ushort)addr);
            }
        }

        /// <summary>
        /// Set the machnine status
        /// </summary>
        protected override void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            if (VmRuns)
            {
                if (MachineViewModel.IsFirstStart)
                {
                    // --- We have just started the virtual machine
                    SetFullView();
                    InitMemoryLines();
                }
                RefreshMemoryLines();
            }

            // --- ... or paused.
            else if (VmPaused)
            {
                MessengerInstance.Send(new RefreshMemoryViewMessage());
            }

            // --- We clear the memory contents as the virtual machine is stopped.
            else if (VmStopped)
            {
                SetRomView(0);
            }
        }

        /// <summary>
        /// We refresh the memory view for every 25th screen refresh
        /// </summary>
        protected override void OnScreenRefreshed()
        {
            if (ScreenRefreshCount % 25 == 0)
            {
                MessengerInstance.Send(new RefreshMemoryViewMessage());
            }
        }

        /// <summary>
        /// Override this method to handle the solution closed event
        /// </summary>
        protected override void OnSolutionClosed(SolutionClosedMessage msg)
        {
            base.OnSolutionClosed(msg);
            MemoryLines.Clear();
        }

        /// <summary>
        /// Initializes the memory lines with empty values
        /// </summary>
        private void InitMemoryLines()
        {
            var memory = GetMemoryBuffer();
            var length = GetMemoryLength();
            if (memory == null || length == null) return;

            MemoryLines.Clear();
            for (var i = 0; i < length; i+= 16)
            {
                var line = new MemoryLineViewModel((ushort)i);
                line.BindTo(memory);
                MemoryLines.Add(line);
            }
        }

        /// <summary>
        /// Retrieves the memory buffer to show
        /// </summary>
        private byte[] GetMemoryBuffer()
        {
            return FullViewMode
                ? MachineViewModel?.SpectrumVm?.MemoryDevice?.CloneMemory()
                : (RomViewMode
                    ? MachineViewModel?.SpectrumVm?.MemoryDevice?.GetRomBuffer(RomIndex)
                    : MachineViewModel?.SpectrumVm?.MemoryDevice?.GetRamBank(RamBankIndex));
        }

        /// <summary>
        /// Retrieves the length of the memory buffer to show
        /// </summary>
        private int? GetMemoryLength()
        {
            return FullViewMode
                ? MachineViewModel?.SpectrumVm?.MemoryDevice?.AddressableSize
                : MachineViewModel?.SpectrumVm?.MemoryDevice?.PageSize;
        }
    }
}