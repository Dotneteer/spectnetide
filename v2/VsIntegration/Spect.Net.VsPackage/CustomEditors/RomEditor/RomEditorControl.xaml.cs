using Microsoft.VisualStudio.ProjectSystem.References;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.VsPackage.ToolWindows;
using Spect.Net.VsPackage.ToolWindows.BankAware;
using Spect.Net.VsPackage.ToolWindows.BasicList;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// This class defines the user control that displays the ROM
    /// </summary>
    public partial class RomEditorControl
    {
        private MemoryViewModel _vm;

        /// <summary>
        /// The view model that represents the ROM
        /// </summary>
        public MemoryViewModel Vm
        {
            get => _vm;
            set => DataContext = _vm = value;
        }

        /// <summary>
        /// The optional view model of the BASIC listing
        /// </summary>
        public BasicListViewModel BasicList { get; set; }

        public RomEditorControl()
        {
            InitializeComponent();
            PreviewKeyDown += (s, e) => MemoryDumpListBox.HandleListViewKeyEvents(e);
            Prompt.CommandLineEntered += OnCommandLineEntered;
            DataContextChanged += (s, e) => Vm = DataContext as MemoryViewModel;
        }

        /// <summary>
        /// When a valid address is provided, we scroll the memory window to that address
        /// </summary>
        private void OnCommandLineEntered(object sender, CommandLineEventArgs e)
        {
            void SignInvalidCommand()
            {
                Prompt.IsValid = false;
                Prompt.ValidationMessage = "Invalid command syntax";
                e.Handled = false;
            }

            var parser = new RomEditorCommandParser(e.CommandLine);
            switch (parser.Command)
            {
                case RomEditorCommandType.Invalid:
                    SignInvalidCommand();
                    return;

                case RomEditorCommandType.Goto:
                    ScrollToTop(parser.Address);
                    break;

                case RomEditorCommandType.Disassemble:
                    if (!Vm.AllowDisassembly)
                    {
                        SignInvalidCommand();
                        return;
                    }
                    Vm.ShowDisassembly = true;
                    MemoryRow.Height = new GridLength(1, GridUnitType.Star);
                    DisassemblyRow.Height = new GridLength(1, GridUnitType.Star);
                    Vm.Disassembly(parser.Address);
                    break;

                case RomEditorCommandType.ExitDisass:
                    if (!Vm.AllowDisassembly)
                    {
                        SignInvalidCommand();
                        return;
                    }
                    Vm.ShowDisassembly = false;
                    MemoryRow.Height = new GridLength(1, GridUnitType.Star);
                    DisassemblyRow.Height = new GridLength(0);
                    break;

                case RomEditorCommandType.ExportProgram:
                    {
                        if (BasicList == null)
                        {
                            Prompt.IsValid = false;
                            Prompt.ValidationMessage = "No BASIC listing in this block";
                            e.Handled = false;
                            break;
                        }
                        if (DisplayExportBasicListDialog(out var vm))
                        {
                            // --- Export cancelled
                            break;
                        }

                        BasicList.ExportToFile(vm.Filename);
                        ExportBasicListViewModel.LatestFolder =
                            Path.GetDirectoryName(vm.Filename);
                        break;
                    }

                case RomEditorCommandType.ExportDisass:
                    {
                        if (DisplayExportDisassemblyDialog(out var vm, parser.Address, parser.Address2))
                        {
                            // --- Export cancelled
                            break;
                        }

                        var exporter = new DisassemblyExporter(vm, new RomEditorDisassemblyParent());
                        var disassembler = new Z80Disassembler(
                            new List<MemorySection> { new MemorySection(parser.Address, parser.Address2) },
                            Vm.MemoryBuffer);
                        exporter.ExportDisassembly(disassembler);
                        ExportDisassemblyViewModel.LatestFolder =
                            Path.GetDirectoryName(vm.Filename);
                        break;
                    }

                default:
                    e.Handled = false;
                    return;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Scrolls the disassembly item with the specified address into view
        /// </summary>
        /// <param name="address"></param>
        public void ScrollToTop(ushort address)
        {
            address &= 0xFFF7;
            var sw = MemoryDumpListBox.GetScrollViewer();
            sw?.ScrollToVerticalOffset(address / 16.0);
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
        private static bool DisplayExportDisassemblyDialog(out ExportDisassemblyViewModel vm, ushort startAddress, ushort endAddress)
        {
            var exportDialog = new ExportDisassemblyDialog()
            {
                HasMaximizeButton = false,
                HasMinimizeButton = false
            };

            vm = new ExportDisassemblyViewModel
            {
                Filename = Path.Combine(ExportDisassemblyViewModel.LatestFolder
                    ?? "C:\\Temp", "ExportedCode.z80asm"),
                StartAddress = startAddress.ToString(),
                EndAddress = endAddress.ToString(),
                AddToProject = true,
                HangingLabels = true,
                CommentStyle = CommentStyle.Semicolon,
                MaxLineLengthType = LineLengthType.L100,
                IndentDepth = IndentDepthType.Two
            };
            exportDialog.SetVm(vm);
            var accepted = exportDialog.ShowModal();
            return !accepted.HasValue || !accepted.Value;
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
        private static bool DisplayExportBasicListDialog(out ExportBasicListViewModel vm)
        {
            var exportDialog = new ExportBasicListDialog()
            {
                HasMaximizeButton = false,
                HasMinimizeButton = false
            };

            vm = new ExportBasicListViewModel
            {
                Filename = Path.Combine(ExportBasicListViewModel.LatestFolder
                    ?? "C:\\Temp", "BasicList.bas"),
            };
            exportDialog.SetVm(vm);
            var accepted = exportDialog.ShowModal();
            return !accepted.HasValue || !accepted.Value;
        }
    }

    /// <summary>
    /// Parent class for disassembly export
    /// </summary>
    class RomEditorDisassemblyParent : IDisassemblyItemParent
    {
        public bool GetComment(ushort address, out string comment)
        {
            comment = string.Empty;
            return false;
        }

        public bool GetLabel(ushort address, out string label)
        {
            label = string.Empty;
            return false;
        }

        public bool GetLiteralReplacement(ushort address, out string symbol)
        {
            symbol = string.Empty;
            return false;
        }

        public bool GetPrefixComment(ushort address, out string comment)
        {
            comment = string.Empty;
            return false;
        }

        public bool HasBreakpoint(ushort address)
        {
            return false;
        }

        public bool HasCondition(ushort itemAddress)
        {
            return false;
        }

        public bool IsCurrentInstruction(ushort address)
        {
            return false;
        }

        public void ToggleBreakpoint(DisassemblyItemViewModel item)
        {
        }
    }
}
