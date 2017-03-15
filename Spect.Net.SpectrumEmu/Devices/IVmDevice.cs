namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// This interface describes a ZX Spectrum VM device
    /// </summary>
    public interface IVmDevice
    {
        /// <summary>
        /// Resets this device
        /// </summary>
        void Reset();
    }
}