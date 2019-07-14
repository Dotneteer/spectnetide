using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command debugs the default program within the active project.
    /// </summary>
    [CommandId(0x0807)]
    public class DebugDefaultProgramCommand : DebugProgramCommand
    {
        public override bool AllowProjectItem => true;
    }
}