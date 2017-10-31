using System.Threading.Tasks;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Set the specified tape file as the default one to load
    /// </summary>
    [CommandId(0x0803)]
    public class SetAsDefaultTapeFileCommand : TapeCommandBase
    {
        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override Task ExecuteAsync()
        {
            Package.CodeDiscoverySolution.CurrentProject.SetDefaultTapeItem(this);
            return Task.FromResult(0);
        }
    }
}