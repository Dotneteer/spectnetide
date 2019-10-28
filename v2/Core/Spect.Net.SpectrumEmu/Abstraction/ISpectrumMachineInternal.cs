using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Abstraction
{
    /// <summary>
    /// This class represents the internal API of a ZX Spectrum virtual machine.
    /// </summary>
    public interface ISpectrumMachineInternal
    {
        /// <summary>
        /// Gets the ISpectrumVm instance behind the machine
        /// </summary>
        ISpectrumVm SpectrumVm { get; }
    }
}