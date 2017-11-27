using System.Collections.ObjectModel;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// This view model represents the ZX Spectrum memory
    /// </summary>
    public class MemoryToolWindowViewModel : BankAwareToolWindowViewModelBase
    {
        public ObservableCollection<MemoryLineViewModel> MemoryLines { get; } =
            new ObservableCollection<MemoryLineViewModel>();

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

            if (VmNotStopped)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                InitViewMode();
            }
        }

        /// <summary>
        /// Refreshes the specified memory line
        /// </summary>
        /// <param name="addr">Address of the memory line</param>
        public override void RefreshItem(int addr)
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
        public override void RefreshViewMode()
        {
            var memory = GetMemoryBuffer();
            var length = GetMemoryLength();
            if (memory == null || length == null) return;

            for (var addr = 0x0000; addr < length; addr += 16)
            {
                RefreshItem((ushort)addr);
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
        protected override void OnSolutionClosed()
        {
            MemoryLines.Clear();
            base.OnSolutionClosed();
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
            const string INV_S48_COMMAND = "This command cannot be used for a Spectrum 48K model.";
            const string INV_RUN_COMMAND = "This command can only be used when the virtual machine is running.";

            // --- Prepare command handling
            validationMessage = null;
            topAddress = null;
            var isSpectrum48 = SpectNetPackage.IsSpectrum48Model();
            var banks = MachineViewModel.SpectrumVm.MemoryConfiguration.RamBanks;
            var roms = MachineViewModel.SpectrumVm.RomConfiguration.NumberOfRoms;

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
                    if (isSpectrum48)
                    {
                        validationMessage = INV_S48_COMMAND;
                        return false;
                    }
                    if (parser.Address > roms - 1)
                    {
                        validationMessage = $"This machine does not have a ROM bank #{parser.Address}";
                        return false;
                    }
                    SetRomViewMode(parser.Address);
                    topAddress = 0;
                    break;

                case MemoryCommandType.SetRamBank:
                    if (isSpectrum48)
                    {
                        validationMessage = INV_S48_COMMAND;
                        return false;
                    }
                    if (VmStopped)
                    {
                        validationMessage = INV_RUN_COMMAND;
                        return false;
                    }
                    if (parser.Address > banks - 1)
                    {
                        validationMessage = $"This machine does not have a RAM bank #{parser.Address}";
                        return false;

                    }
                    SetRamBankViewMode(parser.Address);
                    topAddress = 0;
                    break;

                case MemoryCommandType.MemoryMode:
                    if (isSpectrum48)
                    {
                        validationMessage = INV_S48_COMMAND;
                        return false;
                    }
                    if (VmStopped)
                    {
                        validationMessage = INV_RUN_COMMAND;
                        return false;
                    }
                    SetFullViewMode();
                    break;

                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Initializes the memory lines with empty values
        /// </summary>
        public override void InitViewMode()
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
    }
}