using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command makes the specified virtual floppy disk write protected.
    /// </summary>
    [CommandId(0x0821)]
    public class WriteProtectFloppyCommand : VfddCommandBase
    {
    }
}