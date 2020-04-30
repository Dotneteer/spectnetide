using Spect.Net.VsPackage.VsxLibrary.Output;
using System;
namespace Spect.Net.VsPackage.VsxLibrary
{
    public static class MemoryUtils
    {
        public static void ReportAllocatedMemory()
        {
            var pane = OutputWindow.GetPane<SpectNetIdeOutputPane>();
            pane.WriteLine($"Memory usage: {GC.GetTotalMemory(false)}");
        }
    }
}
