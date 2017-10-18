namespace Spect.Net.SpectrumEmu.Devices.Tape
{
    /// <summary>
    /// Represetns the data in the tape
    /// </summary>
    public interface ITapeData
    {
        /// <summary>
        /// Block Data
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// Pause after this block (given in milliseconds)
        /// </summary>
        ushort PauseAfter { get; }
    }
}