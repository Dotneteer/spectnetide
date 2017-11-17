using Spect.Net.SpectrumEmu.Devices.Screen;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the device that renders the screen
    /// </summary>
    public interface IScreenDevice : ISpectrumBoundDevice, IFrameBoundDevice
    {
        /// <summary>
        /// Gest the parameters of the display
        /// </summary>
        ScreenConfiguration ScreenConfiguration { get; }

        /// <summary>
        /// Table of ULA tact action information entries
        /// </summary>
        RenderingTact[] RenderingTactTable { get; }

        /// <summary>
        /// Indicates the refresh rate calculated from the base clock frequency
        /// of the CPU and the screen configuration (total #of ULA tacts per frame)
        /// </summary>
        decimal RefreshRate { get; }

        /// <summary>
        /// The number of frames when the flash flag should be toggles
        /// </summary>
        int FlashToggleFrames { get; }

        /// <summary>
        /// Executes the ULA rendering actions between the specified tacts
        /// </summary>
        /// <param name="fromTact">First ULA tact</param>
        /// <param name="toTact">Last ULA tact</param>
        void RenderScreen(int fromTact, int toTact);

        /// <summary>
        /// Gets the memory contention value for the specified tact
        /// </summary>
        /// <param name="tact">ULA tact</param>
        /// <returns></returns>
        byte GetContentionValue(int tact);

        /// <summary>
        /// Gets the buffer that holds the screen pixels
        /// </summary>
        /// <returns>Pixel buffer</returns>
        byte[] GetPixelBuffer();
    }
}