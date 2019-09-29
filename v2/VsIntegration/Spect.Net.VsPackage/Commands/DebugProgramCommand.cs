using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.VsxLibrary.Command;
using System;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Debugs the specified code file
    /// </summary>
    [CommandId(0x0801)]
    public class DebugProgramCommand : RunProgramCommand
    {
        /// <summary>
        /// Override this command to start the ZX Spectrum virtual machine
        /// </summary>
        protected override void ResumeVm()
        {
            var vm = HostPackage.EmulatorViewModel;
            vm.MemViewPoint = (ushort)MemoryStartAddress;
            vm.DisAssViewPoint = (ushort)DisassemblyStartAddress;
            vm.StackDebugSupport.ClearStepOutStack();
            vm.Machine.StartDebug();
        }

        /// <summary>
        /// Override this method to prepare assembler options
        /// </summary>
        /// <returns>Options to use with the assembler</returns>
        protected override AssemblerOptions PrepareAssemblerOptions()
        {
            var options = base.PrepareAssemblerOptions();
            var debugOptions = SpectNetPackage.Default.Options.DebugSymbols;
            if (debugOptions != null)
            {
                var symbols = debugOptions.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var symbol in symbols)
                {
                    if (!options.PredefinedSymbols.Contains(symbol))
                    {
                        options.PredefinedSymbols.Add(symbol);
                    }
                }
            }
            return options;
        }
    }
}