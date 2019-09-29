using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Runs the specified code file
    /// </summary>
    [CommandId(0x0800)]
    public class RunProgramCommand : InjectProgramCommand
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
            vm.Machine.Start();
        }

        /// <summary>
        /// Indicates that this command uses the virtual machine in 
        /// code inject mode
        /// </summary>
        protected override bool IsInInjectMode => false;

        /// <summary>
        /// Allows defining a new continuation point
        /// </summary>
        /// <returns>
        /// Address of the continuation point, if a new one should be used;
        /// null, to carry on from the previous point
        /// </returns>
        protected override ushort? GetContinuationAddress() => (ushort)StartAddress;
    }
}