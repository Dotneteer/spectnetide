using Spect.Net.SpectrumEmu.Keyboard;

namespace Spect.Net.SpectrumEmu.Abstraction
{
    /// <summary>
    /// This interface represents the Spectrum keyboard device
    /// </summary>
    public interface IKeyboardDevice : ISpectrumBoundDevice
    {
        /// <summary>
        /// Sets the status of the specified Spectrum keyboard key
        /// </summary>
        /// <param name="key">Key code</param>
        /// <param name="isDown">True, if the key is down; otherwise, false</param>
        void SetStatus(SpectrumKeyCode key, bool isDown);

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