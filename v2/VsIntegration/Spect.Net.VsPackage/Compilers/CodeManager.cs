using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Machine;
using System;

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the events related to code management
    /// </summary>
    public class CodeManager
    {
        /// <summary>
        /// This event signs that code has been injected into the virtual machine.
        /// </summary>
        public event EventHandler CodeInjected;

        /// <summary>
        /// Signs that the compilation has completed
        /// </summary>
        public event EventHandler<CompilationCompletedEventArgs> CompilationCompleted;

        /// <summary>
        /// Signs that the annotation file has been changed
        /// </summary>
        public event EventHandler AnnotationFileChanged;

        /// <summary>
        /// Signs that the code compilation completed.
        /// </summary>
        /// <param name="output">Assembler output</param>
        public void RaiseCompilationCompleted(AssemblerOutput output)
        {
            CompilationCompleted?.Invoke(this, new CompilationCompletedEventArgs(output));
        }

        /// <summary>
        /// Signs that the code has been injected into the virtual machine
        /// </summary>
        public void RaiseCodeInjected()
        {
            CodeInjected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Signs that the annotation file has been changed.
        /// </summary>
        public void RaiseAnnotationFileChanged()
        {
            AnnotationFileChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Injects the code into the Spectrum virtual machine's memory
        /// </summary>
        /// <param name="output"></param>
        public void InjectCodeIntoVm(AssemblerOutput output)
        {
            // --- Do not inject faulty code
            if (output == null || output.ErrorCount > 0)
            {
                return;
            }

            // --- Do not inject code if memory is not available
            var vm = SpectNetPackage.Default.EmulatorViewModel;
            var spectrumVm = vm.Machine.SpectrumVm;
            if (vm.MachineState != VmState.Paused || spectrumVm?.MemoryDevice == null)
            {
                return;
            }

            if (spectrumVm is ISpectrumVmRunCodeSupport runSupport)
            {
                // --- Go through all code segments and inject them
                foreach (var segment in output.Segments)
                {
                    var addr = segment.StartAddress + (segment.Displacement ?? 0);
                    runSupport.InjectCodeToMemory((ushort)addr, segment.EmittedCode);
                }

                // --- Prepare the machine for RUN mode
                runSupport.PrepareRunMode();
                CodeInjected?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
