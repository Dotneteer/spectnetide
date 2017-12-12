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

        /// <summary>
        /// Gets a property of the ROM (depends on virtual machine model)
        /// </summary>
        /// <typeparam name="TProp">Property type</typeparam>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value if found</param>
        /// <param name="romIndex">Index of the ROM, by default, 0</param>
        /// <returns>True, if found; otherwise, false</returns>
        bool GetProperty<TProp>(string key, out TProp value, int romIndex = 0);
    }
}