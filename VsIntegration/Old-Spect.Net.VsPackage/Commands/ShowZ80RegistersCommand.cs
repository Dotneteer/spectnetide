using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.RegistersTool;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the Z80 Registers tool window
    /// </summary>
    [CommandId(0x1100)]
    [ToolWindow(typeof(RegistersToolWindow))]
    public class ShowZ80RegistersCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CodeDiscoverySolution?.CurrentProject != null;
    }
}