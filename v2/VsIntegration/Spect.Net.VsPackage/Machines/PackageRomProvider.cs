using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.VsPackage.Machines
{
    /// <summary>
    /// This provider returns the RomInfo selected in the current project
    /// </summary>
    public class PackageRomProvider : VmComponentProviderBase, IRomProvider
    {
        private SpectNetPackage _package => SpectNetPackage.Default;

        /// <summary>
        /// Gets the resource name for the specified ROM annotation
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>ROM annotation resource name</returns>
        public string GetAnnotationResourceName(string romName, int page = -1)
            => _package.Solution?.GetAnnotationResourceName(romName, page);

        /// <summary>
        /// Loads the binary contents of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Binary contents of the ROM</returns>
        public byte[] LoadRomBytes(string romName, int page = -1)
            => _package.Solution?.LoadRomBytes(romName, page);

        /// <summary>
        /// Loads the annotations of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Annotations of the ROM in serialized format</returns>
        public string LoadRomAnnotations(string romName, int page = -1)
            => _package.Solution?.LoadRomAnnotation(romName, page);
    }
}
