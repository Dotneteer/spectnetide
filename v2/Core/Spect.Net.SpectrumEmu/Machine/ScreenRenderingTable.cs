using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Represents the screen rendering table of the machine
    /// </summary>
    public sealed class ScreenRenderingTable
    {
        private readonly IScreenDevice _screenDevice;

        public ScreenRenderingTable(IScreenDevice screenDevice)
        {
            _screenDevice = screenDevice;
        }

        /// <summary>
        /// Gets the number of items in the screen rendering table
        /// </summary>
        public int Count => _screenDevice.RenderingTactTable.Length;

        /// <summary>
        /// Gets the specified entry of the screen rendering table
        /// </summary>
        /// <param name="index">Rendering table index</param>
        /// <returns></returns>
        public IRenderingTact this[int index]
            => _screenDevice.RenderingTactTable[index];
    }
}