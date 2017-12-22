using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Run a Z80 program command
    /// </summary>
    [CommandId(0x0800)]
    public class RunZ80CodeCommand : InjectZ80CodeCommand
    {
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