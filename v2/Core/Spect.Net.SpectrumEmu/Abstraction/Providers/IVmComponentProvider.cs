using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface defines the abstraction of a component provider
    /// that can inject components into the ZX Spectrum virtual machine
    /// </summary>
    public interface IVmComponentProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        void Reset();

        /// <summary>
        /// The virtual machine that hosts the provider
        /// </summary>
        ISpectrumVm HostVm { get; }

        /// <summary>
        /// Signs that the provider has been attached to the Spectrum virtual machine
        /// </summary>
        void OnAttachedToVm(ISpectrumVm hostVm);
    }
}