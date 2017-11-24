using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Machine;

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

            EvaluateState();
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
        /// Set the machnine status
        /// </summary>
        protected override void OnVmStateChanged(object sender, VmStateChangedEventArgs args)
        {
            if (VmRuns)
            {
                if (MachineViewModel.IsFirstStart)
                {
                    // --- We have just started the virtual machine
                    SetFullViewMode();
                    InitViewMode();
                }
                RefreshViewMode();
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
                SetRomViewMode(0);
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
                    SetRomViewMode(parser.Address);
                    topAddress = 0;
                    break;

                case MemoryCommandType.SetRamBank:
                    SetRamBankViewMode(parser.Address);
                    topAddress = 0;
                    break;

                case MemoryCommandType.MemoryMode:
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