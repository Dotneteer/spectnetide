using System;
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
        /// Indicates if the code has been injected
        /// </summary>
        protected bool CodeInjected { get; private set; }

        /// <summary>
        /// Override this command to start the ZX Spectrum virtual machine
        /// </summary>
        protected virtual void ResumeVm()
        {
            // --- We do not start the machine, just inject the code
        }

        /// <summary>
        /// Indicates that this command uses the virtual machine in 
        /// code inject mode
        /// </summary>
        protected virtual bool IsInInjectMode => true;

        /// <summary>
        /// Allows defining a new continuation point
        /// </summary>
        /// <returns>
        /// Address of the continuation point, if a new one should be used;
        /// null, to carry on from the previous point
        /// </returns>
        protected virtual ushort? GetContinuationAddress() => null;

        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            // --- Prepare the appropriate file to compile/run
            CodeInjected = false;
            GetCodeItem(out var hierarchy, out var itemId);

            // --- Step #1: Compile the code
            if (!CompileCode(hierarchy, itemId)) return;

            // --- Step #2: Check machine compatibility
            var modelName = Package.CodeDiscoverySolution?.CurrentProject?.ModelName;
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

            await SwitchToMainThreadAsync();
            var vm = Package.MachineViewModel;
            var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
            Package.ShowToolWindow<SpectrumEmulatorToolWindow>();

            // --- Step #5: Prepare the machine to be in the appropriate mode
            if (IsInInjectMode)
            {
                if (vm.MachineState == VmState.Running ||
                    vm.MachineState != VmState.Paused && vm.MachineState != VmState.Stopped && vm.MachineState != VmState.None)
                {
                    VsxDialogs.Show("To inject the code into the virtual machine, please pause it first.",
                        "The virtual machine is running");
                    return;
                }
            }
            else
            {
                var stopped = await Package.CodeManager.StopSpectrumVm(options.ConfirmMachineRestart);
                if (!stopped) return;
            }

            // --- Step #6: Start the virtual machine and run it to the injection point
            if (vm.MachineState == VmState.Stopped || vm.MachineState == VmState.None)
            {
                pane.WriteLine("Starting the virtual machine in code injection mode.");

                // --- Use specific startup for each model.
                var started = true;
                try
                {
                    switch (modelName)
                    {
                        case SpectrumModels.ZX_SPECTRUM_48:
                            await Package.StateFileManager.SetSpectrum48StartupState();
                            break;

                        case SpectrumModels.ZX_SPECTRUM_128:
                            if (modelType == SpectrumModelType.Spectrum48)
                            {
                                await Package.StateFileManager.SetSpectrum128In48StartupState();
                            }
                            else
                            {
                                await Package.StateFileManager.SetSpectrum128In128StartupState();
                            }
                            break;

                        case SpectrumModels.ZX_SPECTRUM_P3_E:
                            if (modelType == SpectrumModelType.Spectrum48)
                            {
                                await Package.StateFileManager.SetSpectrumP3In48StartupState();
                            }
                            else
                            {
                                await Package.StateFileManager.SetSpectrumP3InP3StartupState();
                            }
                            break;

                        case SpectrumModels.ZX_SPECTRUM_NEXT:
                            // --- Implement later
                            return;
                        default:
                            // --- Implement later
                            return;
                    }
                }
                catch (Exception)
                {
                    started = false;
                }
                if (!started) return;
            }

            // --- Step #7: Inject the code into the memory, and force
            // --- new disassembly
            pane.WriteLine("Injecting code into the Spectrum virtual machine.");
            Package.CodeManager.InjectCodeIntoVm(Output);
            CodeInjected = true;

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
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override Task FinallyOnMainThread()
        {
            base.FinallyOnMainThread();
            if (IsInInjectMode)
            {
                if (Package.Options.ConfirmInjectCode && CodeInjected)
                {
                    VsxDialogs.Show("The code has been injected.");
                }
            }
            else
            {
                if (Package.Options.ConfirmCodeStart && Output.ErrorCount > 0)
                {
                    VsxDialogs.Show("The code has been started.");
                }
            }
            return Task.FromResult(0);
        }
    }
}