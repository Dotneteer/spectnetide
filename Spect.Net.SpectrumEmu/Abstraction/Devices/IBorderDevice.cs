namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the device that is responsible for
    /// displaying the border color
    /// </summary>
    public interface IBorderDevice : ISpectrumBoundDevice
    {
        /// <summary>
        /// Gets or sets the ULA border color
        /// </summary>
        int BorderColor { get; set; }
    }
}