using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs;
using Spect.Net.VsPackage.Z80Programs.Commands;
using Spect.Net.Wpf.Mvvm;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Run a Z80 program command
    /// </summary>
    [CommandId(0x0800)]
    public class RunZ80CodeCommand : Z80CompileCodeCommandBaseBase
    {
        /// <summary>
        /// Override this command to start the ZX Spectrum virtual machine
        /// </summary>
        protected virtual void ResumeVm()
        {
            Package.MachineViewModel.StartVmCommand.Execute(null);
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

            // --- Step #2: Check for zero code length
            if (Output.Segments.Sum(s => s.EmittedCode.Count) == 0)
            {
                VsxDialogs.Show("The lenght of the compiled code is 0, " +
                    "so there is no code to inject into the virtual machine and run.",
                    "No code to run.");
                return;
            }

            // --- Step #3: Check non-zero displacements
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

            // --- Step #4: Stop the virtual machine if required
            await SwitchToMainThreadAsync();
            Package.ShowToolWindow<SpectrumEmulatorToolWindow>();
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

                // --- Stop the machine and allow 3 frames' time to stop.
                Package.MachineViewModel.StopVmCommand.Execute(null);
                await Task.Delay(60);

                if (vm.VmState != VmState.Stopped)
                {
                    VsxDialogs.Show("The ZX Spectrum virtual machine did not stop.",
                        "Unexpected issue", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                    return;
                }
            }

            // --- Step #5: Start the virtual machine so that later we can load the program
            vm.StartVmWithCodeCommand.Execute(null);

            const int timeOutInSeconds = 5;
            var counter = 0;

            while (vm.VmState != VmState.Paused && counter < timeOutInSeconds * 10)
            {
                await Task.Delay(100);
                counter++;
            }
            if (vm.VmState != VmState.Paused)
            {
                VsxDialogs.Show($"The ZX Spectrum virtual machine did not start within {timeOutInSeconds} seconds.",
                    "Unexpected issue", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                vm.StopVmCommand.Execute(null);
            }

            // --- Step #6: Inject the code into the memory
            Package.CodeManager.InjectCodeIntoVm(Output);

            // --- Step #7: Refresh the disassembly window
            var disassWindow = Package.GetToolWindow<DisassemblyToolWindow>();
            if (disassWindow?.Vm != null)
            {
                disassWindow.Vm.Disassemble();
                Messenger.Default.Send(new RefreshDisassemblyViewMessage());
            }

            // --- Step #8: Jump to execute the code
            vm.SpectrumVm.Cpu.Registers.PC = Output.EntryAddress ?? Output.Segments[0].StartAddress;
            ResumeVm();
        }

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