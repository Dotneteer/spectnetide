using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.StackTool;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the Z80 Cpu Stack tool window
    /// </summary>
    [CommandId(0x1500)]
    [ToolWindow(typeof(StackToolWindow))]
    public class ShowZ80CpuStackCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CodeDiscoverySolution?.CurrentProject != null;
    }
}