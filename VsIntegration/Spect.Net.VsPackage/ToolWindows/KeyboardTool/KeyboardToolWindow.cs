using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// This class implements the Z80 Registers tool window.
    /// </summary>
    [Guid("9B00DABE-2627-49ED-A511-EEE50C624E61")]
    [Caption("ZX Spectrum Keyboard")]
    public class KeyboardToolWindow: 
        SpectrumToolWindowPane<CompoundKeyboardToolWindowControl, KeyboardToolViewModel>
    {
    }
}