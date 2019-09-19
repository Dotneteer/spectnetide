using Spect.Net.VsPackage.VsxLibrary.Output;
using System;

namespace Spect.Net.VsPackage
{
    /// <summary>
    /// This class is responsible to report exceptions that
    /// are not/cannot be handled within SpectNetIde
    /// </summary>
    public static class ExceptionHandler
    {
        public static void Report(Exception ex)
        {
            var pane = OutputWindow.GetPane<SpectNetIdeOutputPane>();
            pane.WriteLine($"Exception occured in SpectNetIde: {ex.Message}\nFull message: ${ex.ToString()}");
        }
    }
}
