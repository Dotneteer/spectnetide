using System.Threading.Tasks;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Commands;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Compiles a Z80 program command
    /// </summary>
    [CommandId(0x0809)]
    public class CompileZ80CodeCommand : Z80CompileCodeCommandBase
    {
        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override Task ExecuteAsync()
        {
            GetCodeItem(out var hierarchy, out var itemId);
            CompileCode(hierarchy, itemId);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override void FinallyOnMainThread()
        {
            base.FinallyOnMainThread();
            if (Package.Options.ConfirmCodeCompile && Output.ErrorCount == 0)
            {
                VsxDialogs.Show("The code has been successfully compiled.");
            }
        }
    }
}