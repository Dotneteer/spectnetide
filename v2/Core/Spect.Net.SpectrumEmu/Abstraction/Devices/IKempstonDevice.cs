namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the Kempston device
    /// </summary>
    public interface IKempstonDevice : ISpectrumBoundDevice
    {
        /// <summary>
        /// Indicates if the Kempston device is present.
        /// </summary>
        bool IsPresent { get; }

        /// <summary>
        /// The flag that indicates if the left button is pressed.
        /// </summary>
        bool LeftPressed { get; }

        /// <summary>
        /// The flag that indicates if the right button is pressed.
        /// </summary>
        bool RightPressed { get; }

        /// <summary>
        /// The flag that indicates if the up button is pressed.
        /// </summary>
        bool UpPressed { get; }

        /// <summary>
        /// The flag that indicates if the down button is pressed.
        /// </summary>
        bool DownPressed { get; }

        /// <summary>
        /// The flag that indicates if the fire button is pressed.
        /// </summary>
        bool FirePressed { get; }
    }
}