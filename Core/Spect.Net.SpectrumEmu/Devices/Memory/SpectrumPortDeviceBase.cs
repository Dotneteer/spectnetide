using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the port device used by the Spectrum virtual machine
    /// </summary>
    /// <remarks>
    /// This class implements I/O contention
    /// </remarks>
    public abstract class SpectrumPortDeviceBase: IPortDevice
    {
        protected IZ80Cpu Cpu;
        protected IScreenDevice ScreenDevice;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public virtual void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            Cpu = hostVm.Cpu;
            ScreenDevice = hostVm.ScreenDevice;
        }

        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Byte read from the port</returns>
        public virtual byte OnReadPort(ushort addr)
        {
            // --- Apply I/O contention delay
            var lowBit = (addr & 0x0001) != 0;
            var ulaHigh = (addr & 0xc000) == 0x4000;
            if (ulaHigh)
            {
                Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact - 1));
            }
            else
            {
                if (!lowBit)
                {
                    Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                }
            }
            return 0xFF;
        }

        /// <summary>
        /// Sends a byte to the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        /// <returns>Byte read from the memory</returns>
        public virtual void OnWritePort(ushort addr, byte data)
        {
            // --- Apply I/O contention delay
            var lowBit = (addr & 0x0001) != 0;
            var ulaHigh = (addr & 0xc000) == 0x4000;
            if (ulaHigh)
            {
                Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact - 1));
            }
            else
            {
                if (!lowBit)
                {
                    Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                }
            }
        }

        /// <summary>
        /// Emulates I/O contention
        /// </summary>
        /// <param name="addr">Contention address</param>
        public void ContentionWait(ushort addr)
        {
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public virtual void Reset()
        {
        }
    }
}