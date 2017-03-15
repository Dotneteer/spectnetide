using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// This interface represents the device that renders the Spectrum VM screen
    /// with the way the ULA chip does
    /// </summary>
    public interface IUlaScreenDevice : IFrameBoundDevice
    {
        /// <summary>
        /// Gets the current frame count
        /// </summary>
        int FrameCount { get; }

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
    }
}