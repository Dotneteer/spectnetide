using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command ejects the specified virtual floppy disk within the active project.
    /// </summary>
    [CommandId(0x0820)]
    public class EjectFloppyCommand : VfddCommandBase
    {
    }
}