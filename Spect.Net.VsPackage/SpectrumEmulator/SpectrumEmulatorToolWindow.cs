using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.SpectrumEmulator
{
    /// <summary>
    /// This class implements the Spectrum emulator tool window.
    /// </summary>
    [Guid("de41a21a-714d-495e-9b3f-830965f9332b")]
    public class SpectrumEmulatorToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumEmulatorToolWindow"/> class.
        /// </summary>
        public SpectrumEmulatorToolWindow() : base(null)
        {
            Caption = "ZX Spectrum Emulator";
            Content = new SpectrumEmulatorToolWindowControl();
        }
    }
}
