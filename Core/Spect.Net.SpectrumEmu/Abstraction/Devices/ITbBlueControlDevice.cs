namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents a TBBlue control device that can be
    /// used to set up TBBLUE registers and their values
    /// </summary>
    public interface ITbBlueControlDevice : IDevice
    {
        /// <summary>
        /// Sets the register index for the next SetRegisterValue operation
        /// </summary>
        /// <param name="index"></param>
        void SetRegisterIndex(byte index);

        /// <summary>
        /// Sets the value of the register specified by the latest
        /// SetRegisterIndex call
        /// </summary>
        /// <param name="value">Register value to set</param>
        void SetRegisterValue(byte value);
    }
}