using Spect.Net.VsPackage.ToolWindows.StackTool;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the Z80 Cpu Stack tool window
    /// </summary>
    [CommandId(0x1500)]
    [ToolWindow(typeof(StackToolWindow))]
    public class ShowZ80CpuStackCommand : SpectrumToolWindowCommandBase
    {
    }
}
