using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.Watch;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the ZX Spectrum Memory tool window
    /// </summary>
    [CommandId(0x1C00)]
    [ToolWindow(typeof(WatchToolWindow))]
    public class ShowWatchMemoryCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CodeDiscoverySolution?.CurrentProject != null;
    }
}