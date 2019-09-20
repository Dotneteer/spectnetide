using Spect.Net.VsPackage.ToolWindows.BasicList;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the BASIC List tool window
    /// </summary>
    [CommandId(0x1600)]
    [ToolWindow(typeof(BasicListToolWindow))]
    public class ShowBasicListCommand : SpectrumToolWindowCommandBase

    {
    }
}
