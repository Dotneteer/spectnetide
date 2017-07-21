namespace Spect.Net.SpectrumEmu.Abstraction
{
    /// <summary>
    /// This interface represents a Spectrum virtual machine
    /// </summary>
    public interface ISpectrumVm : IFrameBoundDevice
    {
        /// <summary>
        /// Gets the frequency of the virtual machine's clock in Hz
        /// </summary>
        int ClockFrequeny { get; }
    }
}