using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Machines;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.ToolWindows.Keyboard;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.Output;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class implements the Spectrum emulator tool window.
    /// </summary>
    [Guid("de41a21a-714d-495e-9b3f-830965f9332b")]
    [Caption("ZX Spectrum Emulator")]
    [ToolWindowToolbar(0x1010)]
    public class SpectrumEmulatorToolWindow :
        SpectrumToolWindowPane<SpectrumEmulatorToolWindowControl, SpectrumEmulatorToolWindowViewModel>
    {
        /// <summary>
        /// VMSTATE file filter string
        /// </summary>
        public const string VMSTATE_FILTER = "VMSTATE Files (*.vmstate)|*.vmstate";

        /// <summary>
        /// Invoke this method from the main thread to initialize toolbar commands.
        /// </summary>
        public static void InitializeToolbarCommands()
        {
            // ReSharper disable ObjectCreationAsStatement
            new StartVmCommand();
            new StopVmCommand();
            new PauseVmCommand();
            new ResetVmCommand();
            new StartDebugVmCommand();
            new StepIntoCommand();
            new StepOverCommand();
            new StepOutCommand();
            new SaveVmStateCommand();
            new LoadVmStateCommand();
            new AddVmStateCommand();
            new ShowKeyboardCommand();
            new ToggleShadowScreenCommand();
            new ToggleUlaIndicationCommand();
            // ReSharper restore ObjectCreationAsStatement
        }

        /// <summary>
        /// Enable scanning the ZX Spectrum keyboard only if this tool window has the focus.
        /// </summary>
        public override void OnActiveFrameChanged(IVsWindowFrame oldFrame, IVsWindowFrame newFrame)
        {
            SpectNetPackage.Default.EmulatorViewModel.EnableKeyboardScan = newFrame == Frame;
        }

        /// <summary>
        /// Refresh the emulator screen
        /// </summary>
        public override void OnFrameIsOnScreenChanged(IVsWindowFrame frame, bool newIsOnScreen)
        {
            if (newIsOnScreen)
            {
                Content.SpectrumControl.ForceRefresh();
            }
        }

        /// <summary>
        /// Gets the virtual machine instance
        /// </summary>
        /// <returns></returns>
        protected override SpectrumEmulatorToolWindowViewModel GetVmInstance()
        {
            return SpectNetPackage.Default.SpectrumViewModel;
        }

        #region Tool Window Commands

        /// <summary>
        /// This class is intended to be the base class of all ZX Spectrum emulator-
        /// specific commands
        /// </summary>
        public abstract class SpectrumVmCommand : SpectNetCommandBase
        {
            /// <summary>
            /// Gets the current ZX Spectrum machine
            /// </summary>
            public SpectrumMachine Machine => SpectNetPackage.Default.EmulatorViewModel.Machine;

            /// <summary>
            /// Gets the current ZX Spectrum machine state
            /// </summary>
            public VmState MachineState => SpectNetPackage.Default.EmulatorViewModel.MachineState;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1081)]
        public class StartVmCommand : SpectrumVmCommand
        {
            protected override void ExecuteOnMainThread()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                Machine.Start();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState != VmState.Running;
        }

        /// <summary>
        /// Stops the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1082)]
        public class StopVmCommand : SpectrumVmCommand
        {
            /// <summary>
            /// Override this method to define the async command body te execute on the
            /// background thread
            /// </summary>
            protected override async Task ExecuteAsync()
            {
                await Machine.Stop();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                mc.Enabled = MachineState == VmState.Running 
                    || MachineState == VmState.Paused;
            }
        }

        /// <summary>
        /// Pauses the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1083)]
        public class PauseVmCommand : SpectrumVmCommand
        {
            /// <summary>
            /// Override this method to define the async command body te execute on the
            /// background thread
            /// </summary>
            protected override async Task ExecuteAsync()
            {
                await Machine.Pause();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Running;
        }

        /// <summary>
        /// Resets the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1084)]
        public class ResetVmCommand : SpectrumVmCommand
        {
            /// <summary>
            /// Override this method to define the async command body te execute on the
            /// background thread
            /// </summary>
            protected override async Task ExecuteAsync()
            {
                await Machine.Stop();
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                Machine.Start();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Running;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        [CommandId(0x1085)]
        public class StartDebugVmCommand : SpectrumVmCommand
        {
            protected override void ExecuteOnMainThread()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                Machine.StartDebug();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState != VmState.Running;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1086)]
        public class StepIntoCommand : SpectrumVmCommand
        {
            protected override async Task ExecuteAsync()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                await Machine.StepInto();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Paused;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1087)]
        public class StepOverCommand : SpectrumVmCommand
        {
            protected override async Task ExecuteAsync()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                await Machine.StepOver();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Paused;
        }

        /// <summary>
        /// Steps out from the current Z80 subroutine call in debug mode
        /// </summary>
        [CommandId(0x1091)]
        public class StepOutCommand : SpectrumVmCommand
        {
            protected override async Task ExecuteAsync()
            {
                Machine.FastTapeMode = SpectNetPackage.Default.Options.UseFastLoad;
                await Machine.StepOut();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Paused;
        }

        /// <summary>
        /// Saves the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1088)]
        public class SaveVmStateCommand : SpectNetCommandBase
        {
            protected override void ExecuteOnMainThread()
            {
                var folder = HostPackage.Options.VmStateSaveFileFolder;
                var filename = VsxDialogs.FileSave(VMSTATE_FILTER, folder);
                if (filename == null) return;

                var spectrum = HostPackage.EmulatorViewModel.Machine.SpectrumVm;
                var state = spectrum.GetVmState(HostPackage.Solution.ActiveProject.ModelName);

                folder = Path.GetDirectoryName(filename);
                if (folder != null && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                File.WriteAllText(filename, state);
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = HostPackage.EmulatorViewModel.MachineState == VmState.Paused;
        }

        /// <summary>
        /// Loads the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1089)]
        public class LoadVmStateCommand : SpectNetCommandBase
        {
            public override bool UpdateUiWhenComplete => true;

            protected override async Task ExecuteAsync()
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var folder = HostPackage.Options.VmStateSaveFileFolder;
                var filename = VsxDialogs.FileOpen(VMSTATE_FILTER, folder);
                if (filename == null) return;

                // --- Stop the virtual machine, provided it runs
                var options = HostPackage.Options;
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                var vm = HostPackage.EmulatorViewModel;
                var machineState = vm.MachineState;
                if ((machineState == VmState.Running || machineState == VmState.Paused))
                {
                    if (options.ConfirmMachineRestart)
                    {
                        var answer = VsxDialogs.Show("Are you sure, you want to restart " +
                            "the ZX Spectrum virtual machine?",
                            "The ZX Spectum virtual machine is running",
                            MessageBoxButton.YesNo, VsxMessageBoxIcon.Question, 1);
                        if (answer == VsxDialogResult.No)
                        {
                            return;
                        }
                    }

                    // --- Stop the machine and allow 50ms to stop.
                    await HostPackage.EmulatorViewModel.Machine.Stop();
                    await Task.Delay(50);

                    if (vm.MachineState != VmState.Stopped)
                    {
                        const string MESSAGE = "The ZX Spectrum virtual machine did not stop.";
                        await pane.WriteLineAsync(MESSAGE);
                        VsxDialogs.Show(MESSAGE, "Unexpected issue",
                            MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                        return;
                    }
                }

                // --- Load the file and keep it paused
                VmStateFileManager.LoadVmStateFile(filename);
                HostPackage.EmulatorViewModel.ForceScreenRefresh();
                HostPackage.EmulatorViewModel.ForcePauseVmAfterStateRestore();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var state = HostPackage.EmulatorViewModel.MachineState;
                mc.Enabled = state == VmState.Paused || state == VmState.Stopped || state == VmState.None;
            }
        }

        /// <summary>
        /// Loads the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1090)]
        public class AddVmStateCommand : SpectrumVmCommand
        {
            private const string FILE_EXISTS_MESSAGE =
                "The virtual machine state file exists in the project. " +
                "Would you like to override it?";

            private const string INVALID_FOLDER_MESSAGE = 
                "The virtual machine state folder specified in the Options dialog " +
                "contains invalid characters or an absolute path. Go to the Options dialog and " +
                "fix the issue so that you can add the virtual machine state file to the project.";

            protected override async Task ExecuteAsync()
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                if (AddVmStateParameterDialog(out var vm)) return;

                // --- Save the file into the vmstate folder
                var filename = Path.Combine(HostPackage.Options.VmStateSaveFileFolder, vm.Filename);
                filename = Path.ChangeExtension(filename, ".vmstate");

                var spectrum = HostPackage.EmulatorViewModel.Machine.SpectrumVm;
                var state = spectrum.GetVmState(HostPackage.Solution.ActiveProject.ModelName);

                var folder = Path.GetDirectoryName(filename);
                if (folder != null && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                File.WriteAllText(filename, state);

                SpectrumProject.AddFileToProject(HostPackage.Options.VmStateProjectFolder, filename,
                    INVALID_FOLDER_MESSAGE, FILE_EXISTS_MESSAGE);
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = MachineState == VmState.Paused;

            /// <summary>
            /// Displays the Export Z80 Code dialog to collect parameter data
            /// </summary>
            /// <param name="vm">View model with collected data</param>
            /// <returns>
            /// True, if the user stars export; false, if the export is cancelled
            /// </returns>
            private bool AddVmStateParameterDialog(out AddVmStateViewModel vm)
            {
                var exportDialog = new AddVmStateDialog
                {
                    HasMaximizeButton = false,
                    HasMinimizeButton = false
                };

                var filename = $"VmState_{DateTime.Now:yyyy_mm_dd_HH_MM_ss}.vmstate";
                vm = new AddVmStateViewModel
                {
                    Filename = filename
                };
                exportDialog.SetVm(vm);
                var accepted = exportDialog.ShowModal();
                if (!accepted.HasValue || !accepted.Value)
                {
                    return true;
                }
                return false;
            }

        }

        /// <summary>
        /// Displays the ZX Spectrum Emulator Keyboard
        /// </summary>
        [CommandId(0x1092)]
        [ToolWindow(typeof(KeyboardToolWindow))]
        public class ShowKeyboardCommand : ShowToolWindowCommandBase
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = true;
        }

        /// <summary>
        /// Enables/Disables the shadow screen
        /// </summary>
        [CommandId(0x1093)]
        public class ToggleShadowScreenCommand : SpectNetCommandBase
        {
            protected EmulatorViewModel Evm => SpectNetPackage.Default.EmulatorViewModel;
            protected override void ExecuteOnMainThread()
            {
                Evm.ShadowScreenEnabled = !Evm.ShadowScreenEnabled;
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Checked = Evm.ShadowScreenEnabled;
        }

        /// <summary>
        /// Enables/Disables the shadow screen
        /// </summary>
        [CommandId(0x1094)]
        public class ToggleUlaIndicationCommand : SpectNetCommandBase
        {
            protected EmulatorViewModel Evm => SpectNetPackage.Default.EmulatorViewModel;
            protected override void ExecuteOnMainThread()
            {
                Evm.UlaIndicationEnabled = !Evm.UlaIndicationEnabled;
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Checked = Evm.UlaIndicationEnabled;
        }

        #endregion
    }
}