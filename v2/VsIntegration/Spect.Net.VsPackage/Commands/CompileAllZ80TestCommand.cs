using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Run Z80 tests command
    /// </summary>
    [CommandId(0x0816)]
    public class CompileAllZ80TestsCommand : Z80TestCommandBase
    {
        /// <summary>
        /// Override this property to allow project item selection
        /// </summary>
        public override bool AllowProjectItem => true;
    }
}