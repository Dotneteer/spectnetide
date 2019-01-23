using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// This class implements the Watch Memory tool window
    /// </summary>
    [Guid("4A07E197-A3FC-4652-8A9C-EC908F439A8A")]
    [Caption("Watch Memory")]
    public class WatchToolWindow:
        SpectrumToolWindowPane<WatchToolWindowControl, WatchToolWindowViewModel>
    {
    }
}