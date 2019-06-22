namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This interface defines the configuration data for the ROM
    /// </summary>
    public interface IRomConfiguration: IDeviceConfiguration
    {
        /// <summary>
        /// The number of ROM banks
        /// </summary>
        int NumberOfRoms { get; }

        /// <summary>
        /// The name of the ROM file
        /// </summary>
        string RomName { get; }

        /// <summary>
        /// The index of the Spectrum 48K BASIC ROM
        /// </summary>
        int Spectrum48RomIndex { get; }
    }
}