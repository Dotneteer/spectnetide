namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents a MultiMedia Card device
    /// </summary>
    public interface IMmcDevice : ISpectrumBoundDevice
    {
        /// <summary>
        /// Selects an MMC card
        /// </summary>
        /// <param name="value">Selector byte</param>
        void SelectCard(byte value);

        /// <summary>
        /// Writes a byte into the MMC control register
        /// </summary>
        /// <param name="value">Value to write into the control register</param>
        void WriteControlRegister(byte value);

        /// <summary>
        /// Reads the value of the control register
        /// </summary>
        /// <returns>Control register value</returns>
        byte ReadControlRegister();
    }
}