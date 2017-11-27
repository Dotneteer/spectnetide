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
        /// Gets the resource name for the specified ROM
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>ROM resource name</returns>
        public string GetRomResourceName(string romName, int page = -1)
            => VsxPackage.GetPackage<SpectNetPackage>()?.CodeDiscoverySolution?
                .GetRomResourceName(romName, page);

        /// <summary>
        /// Gets the resource name for the specified ROM annotation
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>ROM annotation resource name</returns>
        public string GetAnnotationResourceName(string romName, int page = -1)
            => VsxPackage.GetPackage<SpectNetPackage>()?.CodeDiscoverySolution?
                .GetAnnotationResourceName(romName, page);

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