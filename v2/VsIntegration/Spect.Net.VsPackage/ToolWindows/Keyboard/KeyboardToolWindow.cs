using System.Runtime.InteropServices;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.Keyboard
{
    /// <summary>
    /// This class implements the ZX Spectrum Keyboard tool window.
    /// </summary>
    [Guid("9B00DABE-2627-49ED-A511-EEE50C624E61")]
    [Caption("ZX Spectrum Keyboard")]
    public class KeyboardToolWindow :
        SpectrumToolWindowPane<KeyboardToolWindowControl, KeyboardToolWindowViewModel>
    {
        protected override KeyboardToolWindowViewModel GetVmInstance()
        {
            var vm = SpectNetPackage.Default.KeyboardToolModel;
            vm.Initialize();
            return vm;
        }
    }
}