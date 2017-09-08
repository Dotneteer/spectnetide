using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Tools.BasicList
{
    /// <summary>
    /// This class implements the ZX Spectrum BASIC List tool window
    /// </summary>
    [Guid("62C1D5F3-75CA-4E89-A33C-A6F4C628367E")]
    [Caption("BASIC program loaded")]
    public class BasicListToolWindow: SpectrumToolWindowPane<BasicListToolWindowControl>
    {
    }
}