using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Debug a Z80 program command
    /// </summary>
    [CommandId(0x0801)]
    public class DebugZ80CodeCommand : RunZ80CodeCommand
    {
        /// <summary>
        /// Override this command to start the ZX Spectrum virtual machine
        /// </summary>
        protected override void ResumeVm()
        {
            Package.MachineViewModel.StartDebugVmCommand.Execute(null);
        }
    }
}