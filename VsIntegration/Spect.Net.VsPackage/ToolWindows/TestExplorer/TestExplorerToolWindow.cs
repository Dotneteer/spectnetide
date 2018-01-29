using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.TestExplorer
{
    /// <summary>
    /// This class implements the Z80 Registers tool window.
    /// </summary>
    [Guid("D6E4C776-5EFB-48CE-A491-A00A56D89BCA")]
    [Caption("Z80 Unit Test Explorer")]
    public class TestExplorerToolWindow :
        SpectrumToolWindowPane<TestExplorerToolWindowControl, TestExplorerToolWindowViewModel>
    {
    }
}