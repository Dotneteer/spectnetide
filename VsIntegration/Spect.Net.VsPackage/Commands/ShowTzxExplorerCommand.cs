using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.TzxExplorer;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the TZX Explorer tool window
    /// </summary>
    [CommandId(0x1400)]
    [ToolWindow(typeof(TapeFileExplorerToolWindow))]
    public class ShowTzxExplorerCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
    }
}