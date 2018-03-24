using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Vsx.Output;

namespace Spect.Net.VsPackage.Z80Programs
{
    /// <summary>
    /// This class is responsible for managing VM files
    /// </summary>
    public class VmStateFileManager
    {
        /// <summary>
        /// Main Execution cycle loop in Spectrum 48 ROM-0/Spectrum 128 ROM-1
        /// </summary>
        public const ushort SP48_MAIN_EXEC_ADDR = 0x12a9;

        /// <summary>
        /// Main Waiting Loop in Spectrum 128 ROM-0
        /// </summary>
        public const ushort SP128_MAIN_WAITING_LOOP = 0x2653;

        /// <summary>
        /// Return to Editor entry point in Spectrum 128 ROM-0
        /// </summary>
        public const ushort SP128_RETURN_TO_EDITOR = 0x2604;

        /// <summary>
        /// Main Waiting Loop in Spectrum +3E ROM-0
        /// </summary>
        public const ushort SPP3_MAIN_WAITING_LOOP = 0x0706;

        /// <summary>
        /// Return to Editor entry point in Spectrum +3E ROM-0
        /// </summary>
        public const ushort SPP3_RETURN_TO_EDITOR = 0x0937;

        /// <summary>
        /// Time to wait after an emulated menu key has been pressed
        /// </summary>
        public const int WAIT_FOR_MENU_KEY = 250;

        /// <summary>
        /// Number of frames an emulated key is held down
        /// </summary>
        public const int KEY_PRESS_FRAMES = 5;

        public const string VMSTATE_FOLDER = ".SpectNetIde/VmStates";
        public const string SPECTRUM_48_STARTUP = "_sp48.startup.vmstate";
        public const string SPECTRUM_128_STARTUP_48 = "_sp128.startup.48.vmstate";
        public const string SPECTRUM_128_STARTUP_128 = "_sp128.startup.128.vmstate";
        public const string SPECTRUM_P3_STARTUP_48 = "_spP3.startup.48.vmstate";
        public const string SPECTRUM_P3_STARTUP_P3 = "_spP3.startup.P3.vmstate";

        /// <summary>
        /// The package that host the project
        /// </summary>
        public SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// Prepares a Spectrum virtual machine of the current project for code injection
        /// </summary>
        /// <param name="sp48Mode">
        /// Indicates if machine should run in Spectrum 48K mode
        /// </param>
        public async Task<bool> SetProjectMachineStartupState(bool sp48Mode)
        {
            var modelName = Package.CodeDiscoverySolution.CurrentProject.ModelName;
            switch (modelName)
            {
                case SpectrumModels.ZX_SPECTRUM_128:
                    if (sp48Mode)
                    {
                        return await SetSpectrum128In48StartupState();
                    }
                    else
                    {
                        return await SetSpectrum128In128StartupState();
                    }

                case SpectrumModels.ZX_SPECTRUM_P3_E:
                    if (sp48Mode)
                    {
                        return await SetSpectrumP3In48StartupState();
                    }
                    else
                    {
                        return await SetSpectrumP3InP3StartupState();
                    }

                case SpectrumModels.ZX_SPECTRUM_NEXT:
                    return false;

                default:
                    return await SetSpectrum48StartupState();
            }
        }

        /// <summary>
        /// Prepares a Spectrum 48 virtual machine for code injection
        /// </summary>
        public async Task<bool> SetSpectrum48StartupState()
        {
            return await SetSpectrumVmStartupState(SPECTRUM_48_STARTUP, CreateSpectrum48StartupState);
        }

        /// <summary>
        /// Prepares a Spectrum 128 virtual machine for code injection in Spectrum 48 mode
        /// </summary>
        public async Task<bool> SetSpectrum128In48StartupState()
        {
            return await SetSpectrumVmStartupState(SPECTRUM_128_STARTUP_48, CreateSpectrum128Startup48State);
        }

        /// <summary>
        /// Prepares a Spectrum 128 virtual machine for code injection in Spectrum 128 mode
        /// </summary>
        public async Task<bool> SetSpectrum128In128StartupState()
        {
            return await SetSpectrumVmStartupState(SPECTRUM_128_STARTUP_128, CreateSpectrum128Startup128State);
        }

        /// <summary>
        /// Prepares a Spectrum +3 virtual machine for code injection in Spectrum 48 mode
        /// </summary>
        public async Task<bool> SetSpectrumP3In48StartupState()
        {
            return await SetSpectrumVmStartupState(SPECTRUM_P3_STARTUP_48, CreateSpectrumP3Startup48State);
        }

        /// <summary>
        /// Prepares a Spectrum +3 virtual machine for code injection in Spectrum +3 mode
        /// </summary>
        public async Task<bool> SetSpectrumP3InP3StartupState()
        {
            return await SetSpectrumVmStartupState(SPECTRUM_P3_STARTUP_P3, CreateSpectrumP3StartupP3State);
        }

        /// <summary>
        /// Prepres a .vmstate file for loading in the future
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
                && machineState != VmState.BuildingMachine
                && machineState != VmState.Paused
                && machineState != VmState.Pausing)
            {
                throw new InvalidOperationException($"Virtual machine is in an unexpected state: {machineState}");
            }

            // --- Check, if the virtual machine state file exists
            var solution = SpectNetPackage.Default.CodeDiscoverySolution.Root;
            var folder = Path.GetDirectoryName(solution.FileName);
            if (folder == null)
            {
                throw new InvalidOperationException("Project root folder seems to be null.");
            }
            var stateFolder = Path.Combine(folder, VMSTATE_FOLDER);
            var stateFile = Path.Combine(stateFolder, vmFile);
            pane.WriteLine($"Checking VMSTATE file {stateFile}");
            if (File.Exists(stateFile))
            {
                // --- Use the existing state file
                pane.WriteLine($"Loading {stateFile}");
                LoadVmStateFile(stateFile);
                Package.MachineViewModel.Machine.ForceScreenRefresh();
                pane.WriteLine("Virtual machine state restored.");
                return true;
            }

            // --- Create the new virtual machine startup state
            pane.WriteLine("Creating virtual machine startup state.");
            var result = await createAction(vm);

            // --- Save the new state file
            if (result)
            {
                var newState = vm.SpectrumVm.GetVmState(Package.CodeDiscoverySolution.CurrentProject.ModelName);

                if (!Directory.Exists(stateFolder))
                {
                    Directory.CreateDirectory(stateFolder);
                }
                pane.WriteLine($"Saving {stateFile}");
                File.WriteAllText(stateFile, newState);
            }
            return result;
        }

        /// <summary>
        /// Loads the specified .vmstate file
        /// </summary>
        /// <param name="stateFile">Full name of the .vmstate file</param>
        public void LoadVmStateFile(string stateFile)
        {
            var state = File.ReadAllText(stateFile);
            try
            {
                Package.MachineViewModel.SpectrumVm.SetVmState(state, Package.CodeDiscoverySolution.CurrentProject.ModelName);
                Package.MachineViewModel.SpectrumVm.BeeperDevice.Reset();
                Package.MachineViewModel.SpectrumVm.BeeperProvider.Reset();

                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine($"Forcing Paused state from {Package.MachineViewModel.VmState}");
                Package.MachineViewModel.ForcePauseVmAfterStateRestore();
            }
            catch (InvalidVmStateException e)
            {
                VsxDialogs.Show(e.OriginalMessage, "Error loading virtual machine state");
            }
            catch (Exception e)
            {
                VsxDialogs.Show($"Unexpected error: {e.Message}", "Error loading virtual machine state");
            }
        }

        #region Helpers

        /// <summary>
        /// This method recreates the .vmstate file for a Spectrum 48 virtual machine startup
        /// </summary>
        /// <param name="vm">Virtual machine view model</param>
        /// <returns>True, if preparation successful; otherwise, true</returns>
        private async Task<bool> CreateSpectrum48StartupState(MachineViewModel vm)
        {
            var task = vm.RestartVmAndRunToTerminationPoint(0, SP48_MAIN_EXEC_ADDR);
            return await WaitForTerminationPoint(task);
        }

        /// <summary>
        /// This method recreates the .vmstate file for a Spectrum 128 virtual machine 
        /// in Spectrum 48 startup mode
        /// </summary>
        /// <param name="vm">Virtual machine view model</param>
        /// <returns>True, if preparation successful; otherwise, true</returns>
        private async Task<bool> CreateSpectrum128Startup48State(MachineViewModel vm)
        {
            // --- Wait while the main menu appears
            var task = vm.RestartVmAndRunToTerminationPoint(0, SP128_MAIN_WAITING_LOOP);
            if (!await WaitForTerminationPoint(task)) return false;

            task = vm.RunVmToTerminationPoint(1, SP48_MAIN_EXEC_ADDR);

            // --- Move to Spectrum 48 mode
            QueueKeyStroke(SpectrumKeyCode.N6, SpectrumKeyCode.CShift);
            await Task.Delay(WAIT_FOR_MENU_KEY);
            QueueKeyStroke(SpectrumKeyCode.N6, SpectrumKeyCode.CShift);
            await Task.Delay(WAIT_FOR_MENU_KEY);
            QueueKeyStroke(SpectrumKeyCode.N6, SpectrumKeyCode.CShift);
            await Task.Delay(WAIT_FOR_MENU_KEY);
            QueueKeyStroke(SpectrumKeyCode.Enter);
            return await WaitForTerminationPoint(task);
        }

        /// <summary>
        /// This method recreates the .vmstate file for a Spectrum 128 virtual machine 
        /// in Spectrum 128 startup mode
        /// </summary>
        /// <param name="vm">Virtual machine view model</param>
        /// <returns>True, if preparation successful; otherwise, true</returns>
        private async Task<bool> CreateSpectrum128Startup128State(MachineViewModel vm)
        {
            // --- Wait while the main menu appears
            var task = vm.RestartVmAndRunToTerminationPoint(0, SP128_MAIN_WAITING_LOOP);
            if (!await WaitForTerminationPoint(task)) return false;
            task = vm.RunVmToTerminationPoint(0, SP128_RETURN_TO_EDITOR);

            // --- Move to Spectrum 128 mode
            QueueKeyStroke(SpectrumKeyCode.N6, SpectrumKeyCode.CShift);
            await Task.Delay(WAIT_FOR_MENU_KEY);
            QueueKeyStroke(SpectrumKeyCode.Enter);
            return await WaitForTerminationPoint(task);
        }

        /// <summary>
        /// This method recreates the .vmstate file for a Spectrum +3 virtual machine 
        /// in Spectrum 48 startup mode
        /// </summary>
        /// <param name="vm">Virtual machine view model</param>
        /// <returns>True, if preparation successful; otherwise, true</returns>
        private async Task<bool> CreateSpectrumP3Startup48State(MachineViewModel vm)
        {
            // --- Wait while the main menu appears
            var task = vm.RestartVmAndRunToTerminationPoint(0, SPP3_MAIN_WAITING_LOOP);
            if (!await WaitForTerminationPoint(task)) return false;
            task = vm.RunVmToTerminationPoint(3, SP48_MAIN_EXEC_ADDR);

            // --- Move to Spectrum 48 mode
            QueueKeyStroke(SpectrumKeyCode.N6, SpectrumKeyCode.CShift);
            await Task.Delay(WAIT_FOR_MENU_KEY);
            QueueKeyStroke(SpectrumKeyCode.N6, SpectrumKeyCode.CShift);
            await Task.Delay(WAIT_FOR_MENU_KEY);
            QueueKeyStroke(SpectrumKeyCode.N6, SpectrumKeyCode.CShift);
            await Task.Delay(WAIT_FOR_MENU_KEY);
            QueueKeyStroke(SpectrumKeyCode.Enter);
            return await WaitForTerminationPoint(task);
        }

        /// <summary>
        /// This method recreates the .vmstate file for a Spectrum +3 virtual machine 
        /// in Spectrum +3 startup mode
        /// </summary>
        /// <param name="vm">Virtual machine view model</param>
        /// <returns>True, if preparation successful; otherwise, true</returns>
        private async Task<bool> CreateSpectrumP3StartupP3State(MachineViewModel vm)
        {
            // --- Wait while the main menu appears
            var task = vm.RestartVmAndRunToTerminationPoint(0, SPP3_MAIN_WAITING_LOOP);
            if (!await WaitForTerminationPoint(task)) return false;
            task = vm.RunVmToTerminationPoint(0, SPP3_RETURN_TO_EDITOR);

            // --- Move to Spectrum +3 mode
            QueueKeyStroke(SpectrumKeyCode.N6, SpectrumKeyCode.CShift);
            await Task.Delay(WAIT_FOR_MENU_KEY);
            QueueKeyStroke(SpectrumKeyCode.Enter);
            return await WaitForTerminationPoint(task);
        }

        /// <summary>
        /// Waits while the Spectrum virtual machine starts and reaches its termination point
        /// </summary>
        /// <returns>True, if started within timeout; otherwise, false</returns>
        private async Task<bool> WaitForTerminationPoint(Task task)
        {
            const int TIME_OUT_IN_SECONDS = 5;

            await Task.WhenAny(task, Task.Delay(TIME_OUT_IN_SECONDS * 1000));
            if (task.IsCompleted) return true;

            var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
            var message = $"The ZX Spectrum virtual machine did not start within {TIME_OUT_IN_SECONDS} seconds.";
            pane.WriteLine(message);
            VsxDialogs.Show(message, "Unexpected issue", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
            await Package.MachineViewModel.StopVm();
            return false;
        }

        /// <summary>
        /// Enques an emulated key stroke
        /// </summary>
        /// <param name="primaryCode">Primary key code</param>
        /// <param name="secondaryCode">Secondary key code</param>
        private void QueueKeyStroke(SpectrumKeyCode primaryCode,
            SpectrumKeyCode? secondaryCode = null)
        {
            var spectrumVm = Package.MachineViewModel?.SpectrumVm;
            if (spectrumVm == null) return;

            var currentTact = spectrumVm.Cpu.Tacts;
            var lastTact = currentTact + spectrumVm.FrameTacts * KEY_PRESS_FRAMES * spectrumVm.ClockMultiplier;

            Package.MachineViewModel.SpectrumVm.KeyboardProvider.QueueKeyPress(
                new EmulatedKeyStroke(
                    currentTact,
                    lastTact,
                    primaryCode,
                    secondaryCode));
        }

        #endregion
    }
}