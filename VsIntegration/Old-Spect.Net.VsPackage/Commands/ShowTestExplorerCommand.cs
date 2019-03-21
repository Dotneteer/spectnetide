using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.TestExplorer;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the Z80 Unit Test Explorer tool window
    /// </summary>
    [CommandId(0x1900)]
    [ToolWindow(typeof(TestExplorerToolWindow))]
    public class ShowTestExplorerCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CodeDiscoverySolution?.CurrentProject != null;
    }
}