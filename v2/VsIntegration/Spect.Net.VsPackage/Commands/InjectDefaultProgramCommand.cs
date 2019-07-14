using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// This command injects the default program within the active project into the
    /// ZX Spectrum virtual machine.
    /// </summary>
    [CommandId(0x0812)]
    public class InjectDefaultProgramCommand : InjectProgramCommand
    {
    }
}