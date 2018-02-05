using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.CompilerOutput
{
    /// <summary>
    /// This class implements the Z80 Registers tool window.
    /// </summary>
    [Guid("397C5BC8-089D-40BF-90F9-904678939D1E")]
    [Caption("Z80 Assembler Output")]
    public class AssemblerOutputToolWindow : 
        VsxToolWindowPane<AssemblerOutputToolWindowControl, AssemblerOutputToolWindowViewModel>
    {
    }
}