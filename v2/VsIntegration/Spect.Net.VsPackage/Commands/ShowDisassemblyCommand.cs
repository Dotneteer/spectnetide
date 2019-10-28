using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the Z80 Disassembly tool window
    /// </summary>
    [CommandId(0x1200)]
    [ToolWindow(typeof(DisassemblyToolWindow))]
    public class ShowDisassemblyCommand : SpectrumToolWindowCommandBase
    {
    }
}