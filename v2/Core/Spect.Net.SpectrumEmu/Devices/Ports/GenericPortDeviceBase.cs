using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Machine;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    public abstract class GenericPortDeviceBase: IPortDevice
    {
        protected IZ80Cpu Cpu;
        protected IScreenDevice ScreenDevice;

        /// <summary>
        /// List of available handlers
        /// </summary>
        public  List<IPortHandler> Handlers { get; } = new List<IPortHandler>();

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Allows to define a logger for port access
        /// </summary>
        public IPortAccessLogger PortAccessLogger { get; set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public virtual void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            Cpu = hostVm.Cpu;
            ScreenDevice = hostVm.ScreenDevice;
            foreach (var handler in Handlers)
            {
                handler.OnAttachedToVm(hostVm);
                handler.Reset();
            }
        }

        /// <summary>
        /// Reads the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Byte read from the port</returns>
        public virtual byte ReadPort(ushort addr)
        {
            // --- Handle I/O contention
            ContentionWait(addr);

            // --- Find and invoke the handler
            foreach (var handler in Handlers)
            {
                if (!handler.CanRead || (addr & handler.PortMask) != handler.Port) continue;
                if (handler.HandleRead(addr, out var readValue))
                {
                    PortAccessLogger?.PortRead(addr, readValue, true);
                    return readValue;
                }
            }
            var ur = UnhandledRead(addr);
            PortAccessLogger?.PortRead(addr, ur, false);
            return ur;
        }

        /// <summary>
        /// Sends a byte to the port with the specified address
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <param name="data">Data to write to the port</param>
        public virtual void WritePort(ushort addr, byte data)
        {
            // --- Handle I/O contention
            ContentionWait(addr);

            // --- Find and invoke the handler
            var handled = false;
            foreach (var handler in Handlers)
            {
                if (handler.CanWrite && (addr & handler.PortMask) == handler.Port)
                {
                    handler.HandleWrite(addr, data);
                    handled = true;
                }
            }
            PortAccessLogger?.PortWritten(addr, data, handled);
        }

        /// <summary>
        /// Emulates I/O contention
        /// </summary>
        /// <param name="addr">Contention address</param>
        public virtual void ContentionWait(ushort addr)
        {
            var lowBit = (addr & 0x0001) != 0;
            if ((addr & 0xc000) == 0x4000
                || (addr & 0xc000) == 0xC000 && IsContendedBankPagedIn())
            {
                if (lowBit)
                {
                    // --- C:1 x 4 contention scheme
                    Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    Cpu.Delay(1);
                    Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    Cpu.Delay(1);
                    Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    Cpu.Delay(1);
                    Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    Cpu.Delay(1);
                }
                else
                {
                    // --- C:1, C:3 contention scheme
                    Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    Cpu.Delay(1);
                    Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    Cpu.Delay(3);
                }
            }
            else
            {
                if (lowBit)
                {
                    // --- N:4 contention scheme
                    Cpu.Delay(4);
                }
                else
                {
                    // --- N:1, C:3 contention scheme
                    Cpu.Delay(1);
                    Cpu.Delay(ScreenDevice.GetContentionValue(HostVm.CurrentFrameTact));
                    Cpu.Delay(3);
                }
            }
        }

        /// <summary>
        /// Define the test whether a contended RAM is paged in for 0xC000-0xFFFF
        /// </summary>
        /// <returns></returns>
        public virtual bool IsContendedBankPagedIn() => false;

        /// <summary>
        /// Define how to handle an unattached port
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Port value for the unhandled port address</returns>
        public virtual byte UnhandledRead(ushort addr) => 0xFF;

        /// <summary>
        /// Resets this device
        /// </summary>
        public virtual void Reset()
        {
            foreach (var handler in Handlers)
            {
                handler.Reset();
            }
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public virtual IDeviceState GetState() => null;

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public virtual void RestoreState(IDeviceState state)
        {
        }
    }
}