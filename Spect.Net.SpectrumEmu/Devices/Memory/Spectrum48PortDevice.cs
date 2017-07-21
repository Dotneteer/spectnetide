using Spect.Net.SpectrumEmu.Abstraction;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the port device used by the Spectrum 48 virtual machine
    /// </summary>
    public class Spectrum48PortDevice: IPortDevice
    {
        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; }

        public Spectrum48PortDevice(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Byte read from the port</returns>
        public byte OnReadPort(ushort addr)
        {
            if ((addr & 0x0001) != 0) return 0xFF;

            // TODO: Implement these calls
            //var portBits = KeyboardStatus.GetLineStatus((byte)(addr >> 8));
            //var earBit = TapeDevice.GetEarBit(Cpu.Tacts);
            //if (!earBit)
            //{
            //    portBits = (byte)(portBits & 0b1011_1111);
            //}
            //return portBits;
            return 0;
        }

        /// <summary>
        /// Sends a byte to the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        /// <returns>Byte read from the memory</returns>
        public void OnWritePort(ushort addr, byte data)
        {
            if ((addr & 0x0001) == 0)
            {
                // TODO: implement these calls
                //BorderDevice.BorderColor = value & 0x07;
                //BeeperDevice.ProcessEarBitValue((value & 0x10) != 0);
                //TapeDevice.ProcessMicBitValue((value & 0x08) != 0);
            }
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
        }
    }
}