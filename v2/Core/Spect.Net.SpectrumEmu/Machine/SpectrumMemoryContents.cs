using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.TestSupport;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class provides access to the memory contents of the Spectrum virtual 
    /// machine, including reading and writing
    /// </summary>
    public sealed class SpectrumMemoryContents
    {
        private readonly IMemoryDevice _memoryDevice;

        public SpectrumMemoryContents(IMemoryDevice memoryDevice, IZ80Cpu cpu)
        {
            _memoryDevice = memoryDevice;
            if (!(cpu is IZ80CpuTestSupport runSupport))
            {
                throw new ArgumentException("The cpu instance should implement IZ80CpuTestSupport", nameof(cpu));
            }
            ReadTrackingState = new AddressTrackingState(runSupport.MemoryReadStatus);
            WriteTrackingState = new AddressTrackingState(runSupport.MemoryReadStatus);
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

        /// <summary>
        /// Resets the memory read tracking information
        /// </summary>
        public void ResetReadTracking()
        {
            ReadTrackingState.Clear();
        }

        /// <summary>
        /// Resets the memory write tracking information
        /// </summary>
        public void ResetWriteTracking()
        {
            WriteTrackingState.Clear();
        }

        /// <summary>
        /// Gets the memory read tracking state information
        /// </summary>
        public AddressTrackingState ReadTrackingState { get; }

        /// <summary>
        /// Gets the memory write tracking state information
        /// </summary>
        public AddressTrackingState WriteTrackingState { get; }
    }

}