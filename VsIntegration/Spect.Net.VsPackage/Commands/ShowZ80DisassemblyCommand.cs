using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the Z80 Registers tool window
    /// </summary>
    [CommandId(0x1200)]
    [ToolWindow(typeof(DisassemblyToolWindow))]
    public class ShowZ80DisassemblyCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CodeDiscoverySolution?.CurrentProject != null;
    }
}