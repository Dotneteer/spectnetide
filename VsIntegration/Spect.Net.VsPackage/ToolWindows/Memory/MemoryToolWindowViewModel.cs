using System.Collections.ObjectModel;
using Spect.Net.CommandParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Machine;
// ReSharper disable IdentifierTypo

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
        /// Refresh the view mode for every start/continue
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            MemoryLineViewModel.RefreshRegisterBrushes();
        }

        /// <summary>
        /// Override this method to define how to refresh the view 
        /// when the virtual machine is paused
        /// </summary>
        public override void RefreshOnPause()
        {
            base.RefreshOnPause();
            RefreshViewMode();
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

            Registers regs = null;
            if (MachineViewModel != null)
            {
                if (MachineViewModel.MachineState != VmState.None && MachineViewModel.MachineState != VmState.Stopped)
                {
                    regs = MachineViewModel.SpectrumVm.Cpu.Registers;
                }
            }
            var memLine = new MemoryLineViewModel(regs, addr);
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
            // --- Prepare command handling
            validationMessage = null;
            topAddress = null;
            var isSpectrum48 = SpectNetPackage.IsSpectrum48Model();
            var banks = MachineViewModel.SpectrumVm.MemoryConfiguration.RamBanks;
            var roms = MachineViewModel.SpectrumVm.RomConfiguration.NumberOfRoms;

            var command = ParseCommand(commandText);
            if (command is CompactToolCommand compactCommand)
            {
                command = ParseCommand(compactCommand.CommandText);
            }
            if (command == null || command.HasSemanticError)
            {
                validationMessage = "Invalid command syntax";
                return false;
            }

            switch (command)
            {
                case GotoToolCommand gotoCommand:
                    if (gotoCommand.Symbol != null)
                    {
                        if (ResolveSymbol(gotoCommand.Symbol, out var symbolValue))
                        {
                            topAddress = symbolValue;
                            break;
                        }

                        validationMessage = string.Format(UNDEF_SYMBOL, gotoCommand.Symbol);
                        return false;
                    }
                    topAddress = gotoCommand.Address;
                    break;

                case RomPageToolCommand romPageCommand:
                    if (isSpectrum48)
                    {
                        validationMessage = INV_S48_COMMAND;
                        return false;
                    }
                    if (romPageCommand.Page > roms - 1)
                    {
                        validationMessage = $"This machine does not have a ROM bank #{romPageCommand.Page}";
                        return false;
                    }
                    SetRomViewMode(romPageCommand.Page);
                    topAddress = 0;
                    break;

                case BankPageToolCommand bankPageCommand:
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
                    if (bankPageCommand.Page > banks - 1)
                    {
                        validationMessage = $"This machine does not have a RAM bank #{bankPageCommand.Page}";
                        return false;

                    }
                    SetRamBankViewMode(bankPageCommand.Page);
                    topAddress = 0;
                    break;

                case MemoryModeToolCommand _:
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
            Registers regs = null;
            if (MachineViewModel != null)
            {
                if (MachineViewModel.MachineState != VmState.None && MachineViewModel.MachineState != VmState.Stopped)
                {
                    regs = MachineViewModel.SpectrumVm.Cpu.Registers;
                }
            }
            for (var i = 0; i < length; i+= 16)
            {
                var line = new MemoryLineViewModel(regs, (ushort)i);
                line.BindTo(memory);
                MemoryLines.Add(line);
            }
        }
    }
}