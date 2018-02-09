using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Vsx.Output;
using Spect.Net.VsPackage.Z80Programs;
using Task=System.Threading.Tasks.Task;

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class implements the Spectrum emulator tool window.
    /// </summary>
    [Guid("de41a21a-714d-495e-9b3f-830965f9332b")]
    [Caption("ZX Spectrum Emulator")]
    [ToolWindowToolbar(typeof(SpectNetCommandSet), 0x1010)]
    public class SpectrumEmulatorToolWindow : 
        SpectrumToolWindowPane<SpectrumEmulatorToolWindowControl, SpectrumEmulatorToolWindowViewModel>
    {
        /// <summary>
        /// VMSTATE file filter string
        /// </summary>
        public const string VMSTATE_FILTER = "VMSTATE Files (*.vmstate)|*.vmstate";

        /// <summary>
        /// Creates a new view model every time a new solution is opened.
        /// </summary>
        protected override void OnSolutionOpened()
        {
            base.OnSolutionOpened();
            (Content as ISupportsMvvm<SpectrumEmulatorToolWindowViewModel>)
                .SetVm(new SpectrumEmulatorToolWindowViewModel());
        }

        /// <summary>Called when the active IVsWindowFrame changes.</summary>
        /// <param name="oldFrame">The old active frame.</param>
        /// <param name="newFrame">The new active frame.</param>
        public override void OnActiveFrameChanged(IVsWindowFrame oldFrame, IVsWindowFrame newFrame)
        {
            if (Content?.Vm?.MachineViewModel != null)
            {
                Content.Vm.MachineViewModel.AllowKeyboardScan = newFrame == Frame;
            }
        }

        /// <summary>
        /// Obtains the current state of the virtual machine
        /// </summary>
        /// <param name="package">Package instance</param>
        /// <returns>Virtual machine state</returns>
        public static VmState GetVmState(SpectNetPackage package) 
            => package.MachineViewModel?.VmState ?? VmState.None;

        /// <summary>
        /// Prepares the virtual machine execution options
        /// </summary>
        private static void PrepareRunOptions()
        {
            var package = SpectNetPackage.Default;
            var state = GetVmState(package);
            if (state == VmState.None
                || state == VmState.BuildingMachine
                || state == VmState.Stopped)
            {
                package.MachineViewModel.FastTapeMode = package.Options.UseFastLoad;
            }
            package.MachineViewModel.SkipInterruptRoutine = package.Options.SkipInterruptRoutine;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1081)]
        public class StartVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                PrepareRunOptions();
                Package.MachineViewModel.StartVm();
            }

            protected override void OnQueryStatus(OleMenuCommand mc) 
                => mc.Enabled = GetVmState(Package) != VmState.Running;
        }

        /// <summary>
        /// Stops the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1082)]
        public class StopVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute() => 
                Package.MachineViewModel.StopVm();

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var state = GetVmState(Package);
                mc.Enabled = state == VmState.Running
                             || state == VmState.Paused;
            }
        }

        /// <summary>
        /// Pauses the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1083)]
        public class PauseVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.MachineViewModel.PauseVm();

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) == VmState.Running;
        }

        /// <summary>
        /// Resets the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1084)]
        public class ResetVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.MachineViewModel.ResetVm();

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) == VmState.Running;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        [CommandId(0x1085)]
        public class StartDebugVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                PrepareRunOptions();
                Package.MachineViewModel.StartDebugVm();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) != VmState.Running;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1086)]
        public class StepIntoCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                PrepareRunOptions();
                Package.MachineViewModel.StepInto();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) == VmState.Paused;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1087)]
        public class StepOverCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                PrepareRunOptions();
                Package.MachineViewModel.StepOver();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) == VmState.Paused;
        }

        /// <summary>
        /// Saves the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1088)]
        public class SaveVmStateCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                var folder = Package.Options.VmStateSaveFileFolder;
                var filename = VsxDialogs.FileSave(VMSTATE_FILTER, folder);
                if (filename == null) return;

                var spectrum = Package.MachineViewModel.SpectrumVm;
                var state = spectrum.GetVmState(Package.CodeDiscoverySolution.CurrentProject.ModelName);

                folder = Path.GetDirectoryName(filename);
                if (folder != null && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                File.WriteAllText(filename, state);
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) == VmState.Paused;
        }

        /// <summary>
        /// Loads the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1089)]
        public class LoadVmStateCommand :
            VsxAsyncCommand<SpectNetPackage, SpectNetCommandSet>
        {
            /// <summary>
            /// This flags indicates that the command UI should be 
            /// updated when the command has been completed --
            /// with failure or success
            /// </summary>
            public override bool UpdateUiWhenComplete => true;

            /// <summary>
            /// Override this method to define the async command body te execute on the
            /// background thread
            /// </summary>
            protected override async Task ExecuteAsync()
            {
                var folder = Package.Options.VmStateSaveFileFolder;
                var filename = VsxDialogs.FileOpen(VMSTATE_FILTER, folder);
                if (filename == null) return;

                // --- Stop the virtual machine, provided it runs
                var options = Package.Options;
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                var vm = Package.MachineViewModel;
                var machineState = vm.VmState;
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
                    Package.MachineViewModel.StopVm();
                    await Task.Delay(50);

                    if (vm.VmState != VmState.Stopped)
                    {
                        const string MESSAGE = "The ZX Spectrum virtual machine did not stop.";
                        pane.WriteLine(MESSAGE);
                        VsxDialogs.Show(MESSAGE, "Unexpected issue",
                            MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                        return;
                    }
                }

                // --- Load the file and keep it paused
                Package.StateFileManager.LoadVmStateFile(filename);
                Package.MachineViewModel.ForcePauseVm();
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var state = GetVmState(Package);
                mc.Enabled = state == VmState.Paused || state == VmState.Stopped || state == VmState.BuildingMachine;
            }
        }

        /// <summary>
        /// Loads the state of the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1090)]
        public class AddVmStateCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
            }

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var state = GetVmState(Package);
                mc.Enabled = state == VmState.Paused;
            }
        }
    }
}
