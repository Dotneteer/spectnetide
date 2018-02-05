using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.CompilerOutput;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the Z80 Registers tool window
    /// </summary>
    [CommandId(0x1A00)]
    [ToolWindow(typeof(AssemblerOutputToolWindow))]
    public class ShowZ80AssemblerOutputCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CodeDiscoverySolution?.CurrentProject != null;
    }
}