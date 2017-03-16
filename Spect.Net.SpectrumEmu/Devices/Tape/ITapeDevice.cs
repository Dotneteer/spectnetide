namespace Spect.Net.SpectrumEmu.Devices.Tape
{
    /// <summary>
    /// This interface defines the a tape device of the Spectrum VM
    /// </summary>
    public interface ITapeDevice: IVmDevice
    {
        /// <summary>
        /// Gets the EAR bit read from the tape
        /// </summary>
        /// <param name="cpuTicks"></param>
        /// <returns></returns>
        bool GetEarBit(ulong cpuTicks);

        /// <summary>
        /// Sets the current tape mode according to the current PC register
        /// and the MIC bit state
        /// </summary>
        void SetTapeMode();

        /// <summary>
        /// Processes the the change of the MIC bit
        /// </summary>
        /// <param name="micBit"></param>
        void ProcessMicBitValue(bool micBit);
    }
}