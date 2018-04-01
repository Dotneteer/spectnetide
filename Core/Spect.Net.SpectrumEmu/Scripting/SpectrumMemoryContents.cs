using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This class provides access to the memory contents of the Spectrum virtual 
    /// machine, including reading and writing
    /// </summary>
    public sealed class SpectrumMemoryContents
    {
        private readonly IMemoryDevice _memoryDevice;

        public SpectrumMemoryContents(IMemoryDevice memoryDevice)
        {
            _memoryDevice = memoryDevice;
        }

        /// <summary>
        /// Gets or sets the contents of the specified memory address
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <returns></returns>
        public byte this[ushort address]
        {
            get => _memoryDevice.Read(address, true);
            set => _memoryDevice.Write(address, value, true);
        }
    }
}