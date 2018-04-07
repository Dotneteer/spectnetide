using Spect.Net.VsPackage.Vsx;
using System.Threading.Tasks;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Run a Z80 program command
    /// </summary>
    [CommandId(0x0800)]
    public class RunZ80CodeCommand : InjectZ80CodeCommand
    {
        /// <summary>
        /// Override this command to start the ZX Spectrum virtual machine
        /// </summary>
        protected override void ResumeVm()
        {
            Package.MachineViewModel.Start();
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
        protected override ushort? GetContinuationAddress() => (ushort) StartAddress;
    }
}