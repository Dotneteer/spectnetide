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
            set
            {
                if (!Set(ref _fullViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
            }
        }

        /// <summary>
        /// A ROM page is displayed
        /// </summary>
        public bool RomViewMode
        {
            get => _romViewMode;
            set
            {
                if (!Set(ref _romViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
            }
        }

        /// <summary>
        /// A RAM bank page is displayed
        /// </summary>
        public bool RamBankViewMode
        {
            get => _ramBankViewMode;
            set
            {
                if (!Set(ref _ramBankViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
            }
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
        /// Should the ROM tag be displayed?
        /// </summary>
        public bool ShowRomTag => FullViewMode || RomViewMode;

        /// <summary>
        /// Should the BANK tag be displayed?
        /// </summary>
        public bool ShowBankTag => FullViewMode || RamBankViewMode;

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
            if (IsInDesignMode)
            {
                FullViewMode = true;
                return;
            }

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
            UpdatePageInformation();
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
        /// Updates the page information
        /// </summary>
        public void UpdatePageInformation()
        {
            RomIndex = MachineViewModel?.SpectrumVm?.MemoryDevice?.GetSelectedRomIndex() ?? 0;
            RamBankIndex = MachineViewModel?.SpectrumVm?.MemoryDevice?.GetSelectedBankIndex(3) ?? 0;
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
        /// Refreshes the memory line specified by the address if only
        /// if the address is displayed in the current view mode
        /// </summary>
        /// <param name="addr">Address to refresh</param>
        /// <remarks>
        /// If the address is within the ROM or RAM bank area, the currently
        /// displayed ROM or RAM bank index should match with the one used
        /// by the Spectrum virtual machine
        /// </remarks>
        public void RefreshMemoryLineOfCurrentView(int addr)
        {
            if (addr >= 0 && addr <= 0x3FFF && (RomViewMode || RamBankViewMode))
            {
                // --- No need to refresh the ROM or RAM bank view
                return;
            }
            RefreshMemoryLine(addr);
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
                if (FullViewMode)
                {
                    UpdatePageInformation();
                }
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
        /// Processes the command text
        /// </summary>
        /// <param name="commandText">The command text</param>
        /// <param name="validationMessage">
        ///     Null, if the command is valid; otherwise the validation message to show
        /// </param>
        /// <param name="topAddress">
        /// Non-null value indicates that the view should be scrolled to that address
        /// </param>
        /// <returns>
        /// True, if the command has been handled; otherwise, false
        /// </returns>
        public bool ProcessCommandline(string commandText, out string validationMessage, 
            out ushort? topAddress)
        {
            // --- Prepare command handling
            validationMessage = null;
            topAddress = null;

            var parser = new MemoryCommandParser(commandText);
            switch (parser.Command)
            {
                case MemoryCommandType.Invalid:
                    validationMessage = "Invalid command syntax";
                    return false;

                case MemoryCommandType.Goto:
                    topAddress = parser.Address;
                    break;

                case MemoryCommandType.SetRomPage:
                    SetRomView(parser.Address);
                    topAddress = 0;
                    break;

                case MemoryCommandType.SetRamBank:
                    SetRamBankView(parser.Address);
                    topAddress = 0;
                    break;

                case MemoryCommandType.MemoryMode:
                    SetFullView();
                    break;

                default:
                    return false;
            }
            return true;
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