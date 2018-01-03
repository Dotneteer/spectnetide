using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Memory
{
    /// <summary>
    /// This class represents the port device used by the Spectrum +3 virtual machine
    /// </summary>
    public class SpectrumP3PortDevice : IPortDevice
    {
        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Byte read from the port</returns>
        public byte OnReadPort(ushort addr)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sends a byte to the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        /// <returns>Byte read from the memory</returns>
        public void OnWritePort(ushort addr, byte data)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Emulates I/O contention
        /// </summary>
        /// <param name="addr">Contention address</param>
        public void ContentionWait(ushort addr)
        {
            throw new System.NotImplementedException();
        }
    }
}