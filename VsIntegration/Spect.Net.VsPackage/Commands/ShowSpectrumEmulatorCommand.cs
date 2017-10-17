using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the ZX Spectrum emulator tool window
    /// </summary>
    [CommandId(0x1000)]
    [ToolWindow(typeof(SpectrumEmulatorToolWindow))]
    public class ShowSpectrumEmulatorCommand :
        VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
    }
}