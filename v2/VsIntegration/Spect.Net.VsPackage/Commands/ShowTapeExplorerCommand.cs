using Spect.Net.VsPackage.ToolWindows.TapeFileExplorer;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the TZX Explorer tool window
    /// </summary>
    [CommandId(0x1400)]
    [ToolWindow(typeof(TapeFileExplorerToolWindow))]
    public class ShowTapeExplorerCommand : SpectrumToolWindowCommandBase
    {
    }
}
