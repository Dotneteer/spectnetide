using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command loads the specified virtual machine state within the active project.
    /// </summary>
    [CommandId(0x0817)]
    public class LoadVmStateCommand : VmStateCommandBase
    {
    }
}