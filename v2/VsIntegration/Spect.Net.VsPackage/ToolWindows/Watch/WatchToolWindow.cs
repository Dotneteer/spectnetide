using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using System;
using System.Runtime.InteropServices;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// This class implements the Watch Memory tool window
    /// </summary>
    [Guid("4A07E197-A3FC-4652-8A9C-EC908F439A8A")]
    [Caption("Watch Memory")]
    public class WatchToolWindow :
        SpectrumToolWindowPane<WatchToolWindowControl, WatchToolWindowViewModel>
    {
        protected override WatchToolWindowViewModel GetVmInstance()
        {
            return SpectNetPackage.Default.WatchViewModel;
        }
    }
}
