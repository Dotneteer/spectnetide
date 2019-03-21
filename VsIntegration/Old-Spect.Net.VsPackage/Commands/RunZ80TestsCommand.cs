using System.Threading.Tasks;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Commands;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Run Z80 tests command
    /// </summary>
    [CommandId(0x0813)]
    public class RunZ80TestsCommand : Z80TestCommandBase
    {
        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override Task ExecuteAsync()
        {
            return Task.FromResult(0);
        }
    }
}