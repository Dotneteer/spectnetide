using System.Collections.Generic;
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
        /// The ZX Spectrum color palette
        /// </summary>
        IReadOnlyList<uint> SpectrumColors { get; }

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