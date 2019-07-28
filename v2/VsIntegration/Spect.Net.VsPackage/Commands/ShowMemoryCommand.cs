using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ToolWindows.Memory;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the Z80 Disassembly tool window
    /// </summary>
    [CommandId(0x1300)]
    [ToolWindow(typeof(MemoryToolWindow))]
    public class ShowMemoryCommand : ShowToolWindowCommandBase
    {
        protected override void OnQueryStatus(OleMenuCommand mc)
            => mc.Enabled = true;
    }
}