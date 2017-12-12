namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface defines a disassembler provider that obtains the
    /// label name from the ZX Spectrum 48 ROM
    /// </summary>
    public interface ISpectrum48RomLabelProvider
    {
        /// <summary>
        /// Gets the label for the specified address from the ZX Spectrum 48 ROM
        /// </summary>
        /// <param name="address">ROM address</param>
        /// <returns>Label, if found; otherwise, null</returns>
        string GetSpectrum48Label(ushort address);
    }
}