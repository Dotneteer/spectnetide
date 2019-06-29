using System.Runtime.InteropServices;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.Registers
{
    /// <summary>
    /// This class implements the Z80 Registers tool window.
    /// </summary>
    [Guid("6379892a-87a5-4b9b-b98c-1c094501a735")]
    [Caption("Z80 Registers")]
    public class RegistersToolWindow :
        ToolWindowPaneBase<RegistersToolWindowControl, RegistersToolWindowViewModel>
    {
    }
}