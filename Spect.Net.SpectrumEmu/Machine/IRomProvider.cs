namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This interface defines the responsibility of a ROM provider
    /// </summary>
    public interface IRomProvider
    {
        /// <summary>
        /// Gets the content of the ROM specified by its resource name
        /// </summary>
        /// <param name="romResourceName">ROM resource name</param>
        /// <returns>Content of the ROM</returns>
        byte[] LoadRom(string romResourceName);
    }
}