using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the ZX Spectrum emulator tool window
    /// </summary>
    [CommandId(0x1000)]
    [ToolWindow(typeof(SpectrumEmulatorToolWindow))]
    public class ShowSpectrumEmulatorCommand : SpectrumToolWindowCommandBase
    {
    }
}