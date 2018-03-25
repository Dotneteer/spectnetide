using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class represents the port device used by the Spectrum +3 virtual machine
    /// </summary>
    public class SpectrumNextPortDevice : GenericPortDeviceBase
    {
        private readonly SpectrumP3MemoryPagePortHandler _memoryPortHandler;
        private readonly SpectrumP3ExtMemoryPagePortHandler _extMemoryPortHandler;

        public SpectrumNextPortDevice()
        {
            Handlers.Add(new Spectrum48PortHandler());
            _memoryPortHandler = new SpectrumP3MemoryPagePortHandler();
            _extMemoryPortHandler = new SpectrumP3ExtMemoryPagePortHandler();
            _memoryPortHandler.RomLowSelectionChanged += (sender, args) 
                => _extMemoryPortHandler.SelectRomLow = (byte) sender;
            _memoryPortHandler.LastSlot3IndexChanged += (sender, args)
                => _extMemoryPortHandler.LastSlot3Index = (int)sender;
            Handlers.Add(_memoryPortHandler);
            _extMemoryPortHandler.RomHighSelectionChanged += (sender, args)
                => _memoryPortHandler.SelectRomHigh = (byte)sender;
            Handlers.Add(_extMemoryPortHandler);
            Handlers.Add(new NextRegisterSelectPortHandler());
            Handlers.Add(new NextRegisterAccessPortHandler());
            Handlers.Add(new SoundRegisterIndexPortHandler());
            Handlers.Add(new SoundRegisterValuePortHandler());
            Handlers.Add(new DivIdeControlPortHandler());
            Handlers.Add(new MmcControlPortHandler());
            Handlers.Add(new MmcCardSelectPortHandler());
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
        public override IDeviceState GetState() => new SpectrumNextPortDeviceState(this);

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public override void RestoreState(IDeviceState state) => state.RestoreDeviceState(this);

        /// <summary>
        /// The state of the Spectrum Next port device
        /// </summary>
        public class SpectrumNextPortDeviceState : IDeviceState
        {
            public bool PagingMode { get; set; }
            public byte SpecialConfig { get; set; }
            public byte SelectRomLow { get; set; }
            public byte SelectRomHigh { get; set; }
            public bool DiskMotorState { get; set; }
            public bool PrinterPortStrobe { get; set; }
            public int LastSlot3Index { get; set; }

            public SpectrumNextPortDeviceState()
            {
            }

            public SpectrumNextPortDeviceState(SpectrumNextPortDevice device)
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
                if (!(device is SpectrumNextPortDevice spNext)) return;

                spNext._extMemoryPortHandler.PagingMode = PagingMode;
                spNext._extMemoryPortHandler.SpecialConfig = SpecialConfig;
                spNext._memoryPortHandler.SelectRomLow = SelectRomLow;
                spNext._extMemoryPortHandler.SelectRomHigh = SelectRomHigh;
                spNext._extMemoryPortHandler.DiskMotorState = DiskMotorState;
                spNext._extMemoryPortHandler.PrinterPortStrobe = PrinterPortStrobe;
                spNext._memoryPortHandler.LastSlot3Index = LastSlot3Index;
            }
        }
    }
}