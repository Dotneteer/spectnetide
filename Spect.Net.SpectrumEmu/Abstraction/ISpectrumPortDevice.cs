namespace Spect.Net.SpectrumEmu.Abstraction
{
    /// <summary>
    /// This interface represents a port device that can be attached to a 
    /// Spectrum virtual machine
    /// </summary>
    public interface ISpectrumPortDevice : ISpectrumBoundDevice, IPortDevice
    {
    }
}