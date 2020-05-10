using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Dialogs.Export;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.ToolWindows.BasicList
{
    /// <summary>
    /// This class implements the ZX Spectrum BASIC List tool window
    /// </summary>
    [Guid("62C1D5F3-75CA-4E89-A33C-A6F4C628367E")]
    [Caption("BASIC program loaded")]
    [ToolWindowToolbar(0x1610)]
    public class BasicListToolWindow :
        SpectrumToolWindowPane<BasicListToolWindowControl, BasicListToolWindowViewModel>
    {
        /// <summary>
        /// Invoke this method from the main thread to initialize toolbar commands.
        /// </summary>
        public static void InitializeToolbarCommands()
        {
            new ExportBasicCommand();
            new MimicZxBasicCommand();
        }

        protected override BasicListToolWindowViewModel GetVmInstance()
        {
            return SpectNetPackage.Default.BasicListViewModel;
        }

        /// <summary>
        /// Exports the list to a file
        /// </summary>
        [CommandId(0x1630)]
        public class ExportBasicCommand : SpectNetCommandBase
        {
            /// <summary>
            /// Override this method to define the status query action
            /// </summary>
            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var state = SpectNetPackage.Default.EmulatorViewModel.Machine.MachineState;
                mc.Enabled = state == VmState.Running || state == VmState.Paused;
            }

            protected override Task ExecuteAsync()
            {
                var tw = SpectNetPackage.Default.GetToolWindow<BasicListToolWindow>();
                if (tw != null && tw.Vm.List != null)
                {
                    if (DisplayExportBasicListDialog(out var vm, tw.Vm.MimicZxBasic))
                    {
                        // --- Export cancelled
                        return Task.FromResult(0);
                    }
                    var list = tw.Vm.List;
                    list.MimicZxBasic = tw.Vm.MimicZxBasic;
                    list.ProgramLines.Clear();
                    list.DecodeBasicProgram();
                    list.ExportToFile(vm.Filename);
                    ExportBasicListViewModel.LatestFolder = Path.GetDirectoryName(vm.Filename);
                }
                return Task.FromResult(0);

                bool DisplayExportBasicListDialog(out ExportBasicListViewModel vm, bool mimicZxb)
                {
                    var exportDialog = new ExportBasicListDialog()
                    {
                        HasMaximizeButton = false,
                        HasMinimizeButton = false
                    };

                    vm = new ExportBasicListViewModel
                    {
                        Filename = Path.Combine(ExportBasicListViewModel.LatestFolder
                            ?? "C:\\Temp", "BasicList.zxbas"),
                        MimicZxBasic = mimicZxb
                    };
                    exportDialog.SetVm(vm);
                    var accepted = exportDialog.ShowModal();
                    return !accepted.HasValue || !accepted.Value;
                }
            }
        }

        /// <summary>
        /// Turns on or off the Mimic ZX BASIC option
        /// </summary>
        [CommandId(0x1631)]
        public class MimicZxBasicCommand : SpectNetCommandBase
        {
            /// <summary>
            /// Override this method to define the status query action
            /// </summary>
            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var tw = SpectNetPackage.Default.GetToolWindow<BasicListToolWindow>();
                if (tw == null)
                {
                    return;
                }
                mc.Enabled = true;
                mc.Checked = tw.Vm.MimicZxBasic;
            }

            protected override Task ExecuteAsync()
            {
                var tw = SpectNetPackage.Default.GetToolWindow<BasicListToolWindow>();
                if (tw != null)
                {
                    tw.Vm.MimicZxBasic = !tw.Vm.MimicZxBasic;
                    tw.Vm.RefreshBasicList();
                }
                return Task.FromResult(0);
            }
        }

    }
}
