using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command runs the default program within the active project.
    /// </summary>
    [CommandId(0x0806)]
    public class RunDefaultProgramCommand : RunProgramCommand
    {
        /// <summary>
        /// Allows the project node to run the default Z80 code file
        /// </summary>
        public override bool AllowProjectItem => true;
    }
}