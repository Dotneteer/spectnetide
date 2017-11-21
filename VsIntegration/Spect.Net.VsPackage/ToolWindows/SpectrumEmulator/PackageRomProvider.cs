using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This provider returns the RomInfo selected in the current project
    /// </summary>
    public class PackageRomProvider: VmComponentProviderBase, IRomProvider
    {
        /// <summary>
        /// Loads the binary contents of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Binary contents of the ROM</returns>
        public byte[] LoadRomBytes(string romName, int page = -1)
            => VsxPackage.GetPackage<SpectNetPackage>()?.CodeDiscoverySolution?
                .LoadRomBytes(romName, page);

        /// <summary>
        /// Loads the annotations of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Annotations of the ROM in serialized format</returns>
        public string LoadRomAnnotations(string romName, int page = -1) 
            => VsxPackage.GetPackage<SpectNetPackage>()?.CodeDiscoverySolution?
                .LoadRomAnnotation(romName, page);
    }
}