using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command Insert a virtual floppy disk into Drive B:
    /// </summary>
    [CommandId(0x0819)]
    public class InsertDriveBCommand : InsertFloppyCommandBase
    {
    }
}