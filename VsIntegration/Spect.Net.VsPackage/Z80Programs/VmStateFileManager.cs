using System;
using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Scripting;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Vsx.Output;

namespace Spect.Net.VsPackage.Z80Programs
{
    /// <summary>
    /// This class is responsible for managing VM files
    /// </summary>
    public class VmStateFileManager: SpectrumVmStateFileManagerBase
    {
        public const string VMSTATE_FOLDER = ".SpectNetIde/VmStates";

        /// <summary>
        /// The package that host the project
        /// </summary>
        public ISpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// Obtains the current model's name
        /// </summary>
        protected override string ModelName => Package.CodeDiscoverySolution.CurrentProject.ModelName;

        /// <summary>
        /// Obtains the Spectrum virtual machine controller this manager is bound to
        /// </summary>
        protected override ISpectrumVmController VmController => Package.MachineViewModel;

        /// <summary>
        /// Obtains the Spectrum virtual machine this manager is bound to
        /// </summary>
        protected override ISpectrumVm SpectrumVm => Package.MachineViewModel.SpectrumVm;

        /// <summary>
        /// Get the name of the folder to save/load machine state files
        /// </summary>
        /// <param name="vmFile">Name of the .vmstate file within the solution folder</param>
        /// <param name="createAction"></param>
        /// <returns>True, if state successfully restored</returns>
        public async Task<bool> SetSpectrumVmStartupState(string vmFile, Func<MachineViewModel, Task<bool>> createAction)
        {
            var vm = Package.MachineViewModel;
            var machineState = vm.VmState;
            var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();

            // --- We cannot set the desired state if the machine is running
            if (machineState != VmState.Stopped
                && machineState != VmState.None
                && machineState != VmState.Paused
                && machineState != VmState.Pausing)
            {
                throw new InvalidOperationException($"Virtual machine is in an unexpected state: {machineState}");
            }

            // --- Check, if the virtual machine state file exists
        /// <returns></returns>
        protected override string GetStateFolder()
        {
            var solution = SpectNetPackage.Default.CodeDiscoverySolution.Root;
            var folder = Path.GetDirectoryName(solution.FileName);
            if (folder == null)
            {
                throw new InvalidOperationException("Project root folder seems to be null.");
            }
            var stateFolder = Path.Combine(folder, VMSTATE_FOLDER);
            return stateFolder;
        }

        /// <summary>
        /// Forces the virtual machine to paused state
        /// </summary>
        protected override void ForcePausedState() 
            => Package.MachineViewModel.ForcePauseVmAfterStateRestore();

        /// <summary>
        /// Define how to reset devices after load
        /// </summary>
        protected override void ResetDevicesAfterLoad()
        {
            Package.MachineViewModel.SpectrumVm.BeeperDevice.Reset();
            Package.MachineViewModel.SpectrumVm.BeeperProvider.Reset();
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">Message to log</param>
        protected override void LogMessage(string message)
        {
            var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
            pane.WriteLine(message);
        }

        /// <summary>
        /// Responds to the event when an invalid machine state has been detected
        /// </summary>
        /// <param name="e">Exception instance</param>
        protected override void OnInvalidVmMachineStateException(InvalidVmStateException e)
        {
            VsxDialogs.Show(e.OriginalMessage, "Error loading virtual machine state");
        }

        /// <summary>
        /// Responds to the event when an loading the machine state raises an exception
        /// </summary>
        /// <param name="e">Exception instance</param>
        protected override void OnLoadVmException(Exception e)
        {
            VsxDialogs.Show($"Unexpected error: {e.Message}", "Error loading virtual machine state");
        }
    }
}