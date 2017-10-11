using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This provider returns the RomInfo selected in the current project
    /// </summary>
    public class PackageRomProvider: IRomProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Gets the content of the ROM specified by its resource name
        /// </summary>
        /// <param name="romResourceName">ROM resource name</param>
        /// <returns>Content of the ROM</returns>
        public RomInfo LoadRom(string romResourceName) 
            => VsxPackage.GetPackage<SpectNetPackage>()?.CurrentWorkspace?.RomInfo;
    }
}