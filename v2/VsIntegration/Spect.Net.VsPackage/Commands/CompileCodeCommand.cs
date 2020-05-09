using System;
using System.Threading.Tasks;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    [CommandId(0x0809)]
    public class CompileCodeCommand : CompileCodeCommandBase
    {
        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override async Task FinallyOnMainThreadAsync()
        {
            await base.FinallyOnMainThreadAsync();
            GC.Collect(2, GCCollectionMode.Forced);
            if (HostPackage.Options.ConfirmCodeCompile && Output.ErrorCount == 0)
            {
                VsxDialogs.Show("The code has been successfully compiled.");
            }
        }
    }
}