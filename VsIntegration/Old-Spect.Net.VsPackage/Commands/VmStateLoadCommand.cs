using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Commands;
using Task = System.Threading.Tasks.Task;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Spect.Net.VsPackage.Commands
{
    [CommandId(0x0817)]
    public class VmStateLoadCommand: VmStateCommandBase
    {
        /// <summary>
        /// Signs that VM state fiel has been loaded
        /// </summary>
        public bool VmStateLoaded { get; private set; }

        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            VmStateLoaded = false;
            GetItem(out var hierarchy, out var itemId);
            if (!(hierarchy is IVsProject project)) return;
            project.GetMkDocument(itemId, out var itemFullPath);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var vm = Package.MachineViewModel;

            // --- Prepare the machine to be in the appropriate mode
            if (vm.MachineState == VmState.Stopped || vm.MachineState == VmState.None)
            {
                vm.Start();
                await vm.Pause();
            }

            if (vm.MachineState != VmState.Paused)
            {
                VsxDialogs.Show("To load state file into the virtual machine, please pause it first.",
                    "The virtual machine is running");
                return;
            }

            Package.ShowToolWindow<SpectrumEmulatorToolWindow>();
            Package.StateFileManager.LoadVmStateFile(itemFullPath);
            vm.Machine.ForceScreenRefresh();
            VmStateLoaded = true;
        }

        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override Task FinallyOnMainThreadAsync()
        {
            if (VmStateLoaded && Package.Options.ConfirmVmStateLoad)
            {
                VsxDialogs.Show("The VM state file has been loaded.");
            }
            return Task.FromResult(0);
        }

    }
}