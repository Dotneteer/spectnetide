using System.Collections.ObjectModel;
using System.IO;
using Spect.Net.CommandParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.ToolWindows.BankAware;
using Spect.Net.VsPackage.Z80Programs.ExportMemory;
using Z80Registers = Spect.Net.SpectrumEmu.Abstraction.Cpu.Registers;

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
                MemoryLines = new ObservableCollection<MemoryLineViewModel>
                {
                    new MemoryLineViewModel(),
                    new MemoryLineViewModel(),
                    new MemoryLineViewModel(),
                    new MemoryLineViewModel(),
                };
                return;
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

            Z80Registers regs = null;
                if (MachineState != VmState.None && MachineState != VmState.Stopped)
                {
                    regs = SpectrumVm.Cpu.Registers;
                }
            var memLine = new MemoryLineViewModel(regs, addr);
            memLine.BindTo(memory, this);
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
            if (SpectrumVm == null) return;

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
        protected override void OnSolutionClosing()
        {
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
            var isSpectrum48 = SpectNetPackage.IsSpectrum48Model();
            var banks = SpectrumVm.MemoryConfiguration.RamBanks;
            var roms = SpectrumVm.RomConfiguration.NumberOfRoms;

            var command = ParseCommand(commandText);
            if (command is CompactToolCommand compactCommand)
            {
                command = ParseCommand(compactCommand.CommandText);
            }
            if (command == null || command.HasSemanticError)
            {
                validationMessage = INV_SYNTAX;
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
                    if (MachineState == VmState.Stopped)
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
                    if (MachineState == VmState.Stopped)
                    {
                        validationMessage = INV_RUN_COMMAND;
                        return false;
                    }
                    SetFullViewMode();
                    break;

                case ExportToolCommand exportMemoryCommand:
                    {
                        if (!ObtainAddress(exportMemoryCommand.From, null,
                            out var startAddress,
                            out validationMessage))
                        {
                            return false;
                        }
                        if (!ObtainAddress(exportMemoryCommand.To, null,
                            out var endAddress,
                            out validationMessage))
                        {
                            return false;
                        }

                        if (DisplayExportMemoryDialog(out var vm, startAddress, endAddress))
                        {
                            // --- Export cancelled
                            break;
                        }

                        var exporter = new MemoryExporter(vm);
                        exporter.ExportMemory(EmulatorViewModel.Machine.SpectrumVm);
                        ExportMemoryViewModel.LatestFolder = Path.GetDirectoryName(vm.Filename);
                        break;
                    }

                default:
                    validationMessage = string.Format(INV_CONTEXT, "ZX Spectrum Memory window");
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
            Z80Registers regs = null;
            if (MachineState != VmState.None && MachineState != VmState.Stopped)
            {
                regs = SpectrumVm.Cpu.Registers;
            }

            for (var i = 0; i < length; i += 16)
            {
                var line = new MemoryLineViewModel(regs, (ushort) i);
                line.BindTo(memory, this);
                MemoryLines.Add(line);
            }
        }

        /// <summary>
        /// Displays the Export Disassembly dialog to collect parameter data
        /// </summary>
        /// <param name="vm">View model with collected data</param>
        /// <param name="startAddress">Disassembly start address</param>
        /// <param name="endAddress">Disassembly end address</param>
        /// <returns>
        /// True, if the user stars export; false, if the export is cancelled
        /// </returns>
        private static bool DisplayExportMemoryDialog(out ExportMemoryViewModel vm, ushort startAddress, ushort endAddress)
        {
            var exportDialog = new ExportMemoryDialog()
            {
                HasMaximizeButton = false,
                HasMinimizeButton = false
            };

            vm = new ExportMemoryViewModel
            {
                Filename = Path.Combine(ExportMemoryViewModel.LatestFolder
                                        ?? "C:\\Temp", "Memory.bin"),
                StartAddress = startAddress.ToString(),
                EndAddress = endAddress.ToString(),
                AddToProject = true
            };
            exportDialog.SetVm(vm);
            var accepted = exportDialog.ShowModal();
            return !accepted.HasValue || !accepted.Value;
        }
    }
}