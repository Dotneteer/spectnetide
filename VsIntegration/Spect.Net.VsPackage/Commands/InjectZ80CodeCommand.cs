using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Vsx.Output;
using Spect.Net.VsPackage.Z80Programs;
using Spect.Net.VsPackage.Z80Programs.Commands;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Inject a Z80 program command
    /// </summary>
    [CommandId(0x0810)]
    public class InjectZ80CodeCommand : Z80CompileCodeCommandBase
    {
        /// <summary>
        /// Override this command to start the ZX Spectrum virtual machine
        /// </summary>
        protected virtual void ResumeVm()
        {
            Package.MachineViewModel.StartVm();
        }

        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            // --- Prepare the appropriate file to compile/run
            GetCodeItem(out var hierarchy, out var itemId);

            // --- Step #1: Compile the code
            if (!CompileCode(hierarchy, itemId)) return;

            // --- Step #2: Check machine compatibility
            var modelName = SpectNetPackage.Default.CodeDiscoverySolution?.CurrentProject?.ModelName;
            SpectrumModelType modelType;
            if (Output.ModelType == null)
            {
                switch (modelName)
                {
                    case SpectrumModels.ZX_SPECTRUM_48:
                        modelType = SpectrumModelType.Spectrum48;
                        break;
                    case SpectrumModels.ZX_SPECTRUM_128:
                        modelType = SpectrumModelType.Spectrum128;
                        break;
                    case SpectrumModels.ZX_SPECTRUM_P3_E:
                        modelType = SpectrumModelType.SpectrumP3;
                        break;
                    case SpectrumModels.ZX_SPECTRUM_NEXT:
                        modelType = SpectrumModelType.Next;
                        break;
                    default:
                        modelType = SpectrumModelType.Spectrum48;
                        break;
                }
            }
            else
            {
                modelType = Output.ModelType.Value;
            }
            if (!SpectNetPackage.IsCurrentModelCompatibleWith(modelType))
            {
                VsxDialogs.Show("The model type defined in the code is not compatible with the " +
                                "Spectum virtual machine of this project.",
                    "Cannot run code.");
                return;
            }

            // --- Step #3: Check for zero code length
            if (Output.Segments.Sum(s => s.EmittedCode.Count) == 0)
            {
                VsxDialogs.Show("The lenght of the compiled code is 0, " +
                                "so there is no code to inject into the virtual machine and run.",
                    "No code to run.");
                return;
            }

            // --- Step #4: Check non-zero displacements
            var options = Package.Options;
            if (Output.Segments.Any(s => (s.Displacement ?? 0) != 0) && options.ConfirmNonZeroDisplacement)
            {
                var answer = VsxDialogs.Show("The compiled code contains non-zero displacement" +
                                             "value, so the displaced code may fail. Are you sure you want to run the code?",
                    "Non-zero displacement found",
                    MessageBoxButton.YesNo, VsxMessageBoxIcon.Question, 1);
                if (answer == VsxDialogResult.No)
                {
                    return;
                }
            }

            // --- Step #5: Stop the virtual machine if required
            await SwitchToMainThreadAsync();
            Package.ShowToolWindow<SpectrumEmulatorToolWindow>();
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

            // --- Step #6: Start the virtual machine so that later we can load the program
            pane.WriteLine("Starting the virtual machine in code injection mode.");

            // --- Use specific startup for each model.
            bool started;
            switch (modelName)
            {
                case SpectrumModels.ZX_SPECTRUM_48:
                    started = await Package.StateFileManager.SetSpectrum48StartupState();
                    break;

                case SpectrumModels.ZX_SPECTRUM_128:
                    started = modelType == SpectrumModelType.Spectrum48
                        ? await Package.StateFileManager.SetSpectrum128In48StartupState()
                        : await Package.StateFileManager.SetSpectrum128In128StartupState();
                    break;

                case SpectrumModels.ZX_SPECTRUM_P3_E:
                    started = modelType == SpectrumModelType.Spectrum48
                        ? await Package.StateFileManager.SetSpectrumP3In48StartupState()
                        : await Package.StateFileManager.SetSpectrumP3InP3StartupState();
                    break;

                case SpectrumModels.ZX_SPECTRUM_NEXT:
                    // --- Implement later
                    return;
                default:
                    // --- Implement later
                    return;
            }
            if (!started) return;

            // --- Step #7: Inject the code into the memory, and force
            // --- new disassembly
            pane.WriteLine("Injecting code into the Spectrum virtual machine.");
            Package.CodeManager.InjectCodeIntoVm(Output);

            // --- Step #8: Jump to execute the code
            var continuationPoint = GetContinuationAddress();
            if (continuationPoint.HasValue)
            {
                vm.SpectrumVm.Cpu.Registers.PC = continuationPoint.Value;
                pane.WriteLine($"Resuming code execution at address {vm.SpectrumVm.Cpu.Registers.PC:X4}.");
            }
            ResumeVm();
        }

        /// <summary>
        /// Allows defining a new continuation point
        /// </summary>
        /// <returns>
        /// Address of the continuation point, if a new one should be used;
        /// null, to carry on from the previous point
        /// </returns>
        protected virtual ushort? GetContinuationAddress() => null;

        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override void FinallyOnMainThread()
        {
            base.FinallyOnMainThread();
            if (Package.Options.ConfirmCodeStart && Output.ErrorCount == 0)
            {
                VsxDialogs.Show("The code has been started.");
            }
        }
    }
}