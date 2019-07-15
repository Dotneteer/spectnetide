namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the ROM device
    /// </summary>
    public interface IRomDevice : ISpectrumBoundDevice
    {
        /// <summary>
        /// Gets the binary contents of the rom
        /// </summary>
        /// <param name="romIndex">Index of the ROM, by default, 0</param>
        /// <returns>Byte array that represents the ROM bytes</returns>
        byte[] GetRomBytes(int romIndex = 0);

        /// <summary>
        /// Gets a known address of a particular ROM
        /// </summary>
        /// <param name="key">Known address key</param>
        /// <param name="romIndex">Index of the ROM, by default, 0</param>
        /// <returns>Address, if found; otherwise, null</returns>
        ushort? GetKnownAddress(string key, int romIndex = 0);
    }
}