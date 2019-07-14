using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command runs the specified unit test case file within the active project.
    /// </summary>
    [CommandId(0x0813)]
    public class RunUnitTestCommand : UnitTestCommandBase
    {
    }
}