using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This device represents the Spectrum 48 memory device
    /// </summary>
    public sealed class Spectrum48MemoryDevice: ISpectrumMemoryDevice
    {
        private IZ80Cpu _cpu;
        private IScreenDevice _screenDevice;
        private byte[] _memory;

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            _cpu = hostVm.Cpu;
            _screenDevice = hostVm.ScreenDevice;
            _memory = new byte[0x10000];
        }

        /// <summary>
        /// Reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        public byte OnReadMemory(ushort addr)
        {
            var value = _memory[addr];
            if ((addr & 0xC000) == 0x4000)
            {
                _cpu.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
            }
            return value;
        }

        /// <summary>
        /// Sets the memory value at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Memory value to write</param>
        /// <returns>Byte read from the memory</returns>
        public void OnWriteMemory(ushort addr, byte value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (addr & 0xC000)
            {
                case 0x0000:
                    // --- ROM cannot be overwritten
                    return;
                case 0x4000:
                    _cpu.Delay(_screenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    break;
            }
            _memory[addr] = value;
        }

        /// <summary>
        /// The ULA reads the memory at the specified address
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>Byte read from the memory</returns>
        /// <remarks>
        /// We need this device to emulate the contention for the screen memory
        /// between the CPU and the ULA.
        /// </remarks>
        public byte OnUlaReadMemory(ushort addr)
        {
            var value = _memory[(addr & 0x3FFF) + 0x4000];
            return value;
        }

        /// <summary>
        /// Fills up the memory from the specified buffer
        /// </summary>
        /// <param name="buffer">Contains the row data to fill up the memory</param>
        /// <param name="startAddress">Z80 memory address to start filling up</param>
        public void FillMemory(byte[] buffer, ushort startAddress)
        {
            buffer?.CopyTo(_memory, startAddress);
        }
    }
}