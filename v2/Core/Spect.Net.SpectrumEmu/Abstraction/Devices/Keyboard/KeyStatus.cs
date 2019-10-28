namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Keyboard
{
    /// <summary>
    /// This structure represents the status of a ZX Spectrum key
    /// </summary>
    public struct KeyStatus
    {
        /// <summary>
        /// Code of a ZX Spectrum key
        /// </summary>
        public readonly SpectrumKeyCode KeyCode;

        /// <summary>
        /// Is the key pressed doen?
        /// </summary>
        public readonly bool IsDown;

        public KeyStatus(SpectrumKeyCode keyCode, bool isDown)
        {
            KeyCode = keyCode;
            IsDown = isDown;
        }
    }
}