using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.Registers;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the ZX Spectrum emulator tool window
    /// </summary>
    [CommandId(0x1100)]
    [ToolWindow(typeof(RegistersToolWindow))]
    public class ShowRegistersCommand : ShowToolWindowCommandBase
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = true;
    }
}