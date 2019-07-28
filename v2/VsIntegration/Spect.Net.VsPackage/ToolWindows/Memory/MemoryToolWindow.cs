using System.Runtime.InteropServices;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// This class implements the Z80 Disassembly tool window.
    /// </summary>
    [Guid("13D0E297-1659-48C0-9502-2B706D13BEF4")]
    [Caption("ZX Spectrum Memory")]
    public class MemoryToolWindow: SpectrumToolWindowPane<MemoryToolWindowControl, MemoryToolWindowViewModel>
    {
    }
}