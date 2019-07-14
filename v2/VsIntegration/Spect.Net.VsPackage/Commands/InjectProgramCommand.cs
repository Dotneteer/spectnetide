using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Injects code in the ZX Spectrum virtual machine
    /// </summary>
    [CommandId(0x0810)]
    public class InjectProgramCommand : CompileCodeCommandBase
    {
    }
}