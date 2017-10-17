using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.Memory;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the ZX Spectrum Memory tool window
    /// </summary>
    [CommandId(0x1300)]
    [ToolWindow(typeof(MemoryToolWindow))]
    public class ShowSpectrumMemoryCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
    }
}