namespace Spect.Net.SpectrumEmu.Abstraction
{
    /// <summary>
    /// Represents a device that is attached to a hosting Spectrum
    /// virtual machine
    /// </summary>
    public interface ISpectrumBoundDevice: IDevice
    {
        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        ISpectrumVm HostVm { get; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        void OnAttachedToVm(ISpectrumVm hostVm);
    }
}