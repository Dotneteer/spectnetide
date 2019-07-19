using Spect.Net.SpectrumEmu.Abstraction.Devices.Keyboard;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the Spectrum keyboard device
    /// </summary>
    public interface IKeyboardDevice : ISpectrumBoundDevice, IRenderFrameBoundDevice
    {
        /// <summary>
        /// Sets the status of the specified Spectrum keyboard key.
        /// </summary>
        /// <param name="keyStatus">Status of the key to set</param>
        void SetStatus(KeyStatus keyStatus);

        /// <summary>
        /// Gets the status of the specified Spectrum keyboard key.
        /// </summary>
        /// <param name="key">Key code</param>
        /// <returns>True, if the key is down; otherwise, false</returns>
        bool GetStatus(SpectrumKeyCode key);

        /// <summary>
        /// Gets the byte we would get when querying the I/O address with the
        /// specified byte as the highest 8 bits of the address line
        /// </summary>
        /// <param name="lines">The highest 8 bits of the address line</param>
        /// <returns>
        /// The status value to be received when querying the I/O
        /// </returns>
        byte GetLineStatus(byte lines);
    }
}