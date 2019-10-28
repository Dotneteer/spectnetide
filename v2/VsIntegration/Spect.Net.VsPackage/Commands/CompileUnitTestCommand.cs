using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command compiles the specified unit test case file within the active project.
    /// </summary>
    [CommandId(0x0814)]
    public class CompileUnitTestCommand : UnitTestCommandBase
    {
    }
}