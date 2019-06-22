using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This class implements a base class for providers
    /// </summary>
    public abstract class VmComponentProviderBase : IVmComponentProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public virtual void Reset()
        {
        }

        /// <summary>
        /// The virtual machine that hosts the provider
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the provider has been attached to the Spectrum virtual machine
        /// </summary>
        public virtual void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }
    }
}