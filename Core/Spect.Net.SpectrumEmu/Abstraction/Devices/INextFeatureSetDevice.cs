namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// Represents a device that implements the Spectrum Next feature set
    /// virtual machine
    /// </summary>
    public interface INextFeatureSetDevice: ISpectrumBoundDevice
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

        /// <summary>
        /// Gets the value of the register specified by the latest
        /// SetRegisterIndex call
        /// </summary>
        /// <remarks>If the specified register is not supported, returns 0xFF</remarks>
        byte GetRegisterValue();
    }
}