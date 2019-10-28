using Spect.Net.VsPackage.ToolWindows.Watch;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the ZX Spectrum Memory tool window
    /// </summary>
    [CommandId(0x1C00)]
    [ToolWindow(typeof(WatchToolWindow))]
    public class ShowWatchMemoryCommand : SpectrumToolWindowCommandBase
    {
    }
}
