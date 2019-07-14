using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command runs all unit test cases within the active project.
    /// </summary>
    [CommandId(0x0815)]
    public class RunAllUnitTestsCommand : UnitTestCommandBase
    {
        /// <summary>
        /// Override this property to allow project item selection
        /// </summary>
        public override bool AllowProjectItem => true;
    }
}