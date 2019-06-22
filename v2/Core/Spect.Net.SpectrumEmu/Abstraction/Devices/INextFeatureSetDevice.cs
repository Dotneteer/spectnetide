namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// Represents a device that implements the Spectrum Next feature set
    /// virtual machine
    /// </summary>
    public interface INextFeatureSetDevice: ISpectrumBoundDevice, ITbBlueControlDevice
    {
        /// <summary>
        /// Gets the value of the register specified by the latest
        /// SetRegisterIndex call
        /// </summary>
        /// <remarks>If the specified register is not supported, returns 0xFF</remarks>
        byte GetRegisterValue();

        /// <summary>
        /// Synchronizes a 16K slot with 8K slots
        /// </summary>
        /// <param name="slotNo16K">Index of 16K slot</param>
        /// <param name="bankNo16K">16K bank to page in</param>
        void Sync16KSlot(int slotNo16K, int bankNo16K);
    }
}