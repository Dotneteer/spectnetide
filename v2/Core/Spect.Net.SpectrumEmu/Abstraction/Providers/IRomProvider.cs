namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface defines the responsibility of a ROM provider
    /// </summary>
    public interface IRomProvider: IVmComponentProvider
    {
        /// <summary>
        /// Loads the binary contents of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Binary contents of the ROM</returns>
        byte[] LoadRomBytes(string romName, int page = -1);

        /// <summary>
        /// Loads the annotations of the ROM.
        /// </summary>
        /// <param name="romName">Name of the ROM</param>
        /// <param name="page">Page of the ROM (-1 means single ROM page)</param>
        /// <returns>Annotations of the ROM in serialized format</returns>
        string LoadRomAnnotations(string romName, int page = -1);
    }
}