using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the port device used by the Spectrum +3 virtual machine
    /// </summary>
    public class SpectrumP3PortDevice : GenericPortDeviceBase
    {
        private readonly SpectrumP3MemoryPagePortHandler _memoryPortHandler;
        private readonly SpectrumP3ExtMemoryPagePortHandler _extMemoryPortHandler;

        public SpectrumP3PortDevice()
        {
            Handlers.Add(new Spectrum48PortHandler(this));
            _memoryPortHandler = new SpectrumP3MemoryPagePortHandler(this);
            _extMemoryPortHandler = new SpectrumP3ExtMemoryPagePortHandler(this);
            _memoryPortHandler.RomLowSelectionChanged += (sender, args) 
                => _extMemoryPortHandler.SelectRomLow = (byte) sender;
            _memoryPortHandler.LastSlot3IndexChanged += (sender, args)
                => _extMemoryPortHandler.LastSlot3Index = (int)sender;
            Handlers.Add(_memoryPortHandler);
            _extMemoryPortHandler.RomHighSelectionChanged += (sender, args)
                => _memoryPortHandler.SelectRomHigh = (byte)sender;
            Handlers.Add(_extMemoryPortHandler);
            Handlers.Add(new SoundRegisterIndexPortHandler(this));
            Handlers.Add(new SoundRegisterValuePortHandler(this));
            Handlers.Add(new SpectrumP3FloatingPointBusPortHandler(this));
            Handlers.Add(new SpectrumP3FloppyStatusPortHandler(this));
            Handlers.Add(new SpectrumP3FloppyCommandPortHandler(this));
        }

        /// <summary>
        /// Ports are not contended.
        /// </summary>
        /// <param name="addr">Contention address</param>
        /// <remarks>
        /// Standard N:4 delay
        /// </remarks>
        public override void ContentionWait(ushort addr)
        {
            Cpu.Delay(4);
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public override IDeviceState GetState() => new SpectrumP3PortDeviceState(this);

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public override void RestoreState(IDeviceState state) => state.RestoreDeviceState(this);

        /// <summary>
        /// The state of the Spectrum +3 port device
        /// </summary>
        public class SpectrumP3PortDeviceState : IDeviceState
        {
            public bool PagingMode { get; set; }
            public byte SpecialConfig { get; set; }
            public byte SelectRomLow { get; set; }
            public byte SelectRomHigh { get; set; }
            public bool DiskMotorState { get; set; }
            public bool PrinterPortStrobe { get; set; }
            public int LastSlot3Index { get; set; }

            public SpectrumP3PortDeviceState()
            {
            }

            public SpectrumP3PortDeviceState(SpectrumP3PortDevice device)
            {
                PagingMode = device._extMemoryPortHandler.PagingMode;
                SpecialConfig = device._extMemoryPortHandler.SpecialConfig;
                SelectRomLow = device._memoryPortHandler.SelectRomLow;
                SelectRomHigh = device._extMemoryPortHandler.SelectRomHigh;
                DiskMotorState = device._extMemoryPortHandler.DiskMotorState;
                PrinterPortStrobe = device._extMemoryPortHandler.PrinterPortStrobe;
                LastSlot3Index = device._memoryPortHandler.LastSlot3Index;
            }

            /// <summary>
            /// Restores the dvice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public void RestoreDeviceState(IDevice device)
            {
                if (!(device is SpectrumP3PortDevice spP3)) return;

                spP3._extMemoryPortHandler.PagingMode = PagingMode;
                spP3._extMemoryPortHandler.SpecialConfig = SpecialConfig;
                spP3._memoryPortHandler.SelectRomLow = SelectRomLow;
                spP3._extMemoryPortHandler.SelectRomHigh = SelectRomHigh;
                spP3._extMemoryPortHandler.DiskMotorState = DiskMotorState;
                spP3._extMemoryPortHandler.PrinterPortStrobe = PrinterPortStrobe;
                spP3._memoryPortHandler.LastSlot3Index = LastSlot3Index;
            }
        }
    }
}