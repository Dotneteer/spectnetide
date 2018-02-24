namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents a TBBlue control device that can be
    /// used to set up TBBLUE registers and their values
    /// </summary>
    public interface ITbBlueControlDevice : IDevice
    {
        /// <summary>
        /// Selects a TBBLUE register that value can be set later
        /// </summary>
        /// <param name="register">Register index</param>
        void SelectTbBlueRegister(byte register);

        /// <summary>
        /// Sets the TBBLUE value of the previously selected register
        /// </summary>
        /// <param name="value">Value to set</param>
        void SetTbBlueValue(byte value);
    }
}