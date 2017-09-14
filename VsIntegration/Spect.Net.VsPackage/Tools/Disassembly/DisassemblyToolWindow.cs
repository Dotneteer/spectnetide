using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// This class implements the Z80 Disassembly tool window.
    /// </summary>
    [Guid("149E947C-6296-4BCE-A939-A5CD3AA6195F")]
    [Caption("Z80 Disassembly")]
    public class DisassemblyToolWindow : 
        SpectrumToolWindowPane<DisassemblyToolWindowControl, DisassemblyToolWindowViewModel>
    {
    }
}