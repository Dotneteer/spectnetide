using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command compiles all unit test cases within the active project.
    /// </summary>
    [CommandId(0x0816)]
    public class CompileAllUnitTestsCommand : UnitTestCommandBase
    {
    }
}