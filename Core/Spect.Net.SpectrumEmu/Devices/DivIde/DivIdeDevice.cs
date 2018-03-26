using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.DivIde
{
    /// <summary>
    /// This class implements a simple divIDE device
    /// </summary>
    public class DivIdeDevice: IDivIdeDevice
    {
        private readonly bool _pagedInAfterReset;
        private IZ80Cpu _cpu;
        private byte _controlReg;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public DivIdeDevice(bool pagedInAfterReset = false)
        {
            _pagedInAfterReset = pagedInAfterReset;
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            _cpu = hostVm.Cpu;
            _cpu.OpcodeFetched += (sender, args) => ProcessOpAddress(_cpu.Registers.PC);
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            _controlReg = 0;
            EnableAutoMapping = false;
            IsDivIdePagedIn = _pagedInAfterReset;
        }

        /// <summary>
        /// Enables/Disables auto mapping
        /// </summary>
        public bool EnableAutoMapping { get; set; }

        /// <summary>
        /// Indicates if DivIDE ROM is paged in
        /// </summary>
        public bool IsDivIdePagedIn { get; private set; }

        /// <summary>
        /// The CONMEM bit of the controller
        /// </summary>
        public bool ConMem => (_controlReg & 0x80) != 0;

        /// <summary>
        /// The MAPRAM bit of the controller
        /// </summary>
        public bool MapRam => (_controlReg & 0x40) != 0;

        /// <summary>
        /// The selected bank (0..3)
        /// </summary>
        public int Bank => _controlReg & 0x03;

        /// <summary>
        /// Sets the divide control register
        /// </summary>
        /// <param name="value">Control register value</param>
        public void SetControlRegister(byte value)
        {
            _controlReg = value;
            IsDivIdePagedIn = ConMem;
        }

        /// <summary>
        /// Processed the specified operation address
        /// </summary>
        /// <param name="addr"></param>
        public void ProcessOpAddress(ushort addr)
        {
            if (!EnableAutoMapping) return;

            if (addr == 0x0000 || addr == 0x0008 || addr == 0x0038
                || addr == 0x0066 || addr == 0x046C || addr == 0x0562 
                || addr >= 0x3D00 && addr <= 0x3DFF)
            {
                IsDivIdePagedIn = true;
                return;
            }

            if (addr >= 0x1FF8 && addr <= 0x1FFF)
            {
                IsDivIdePagedIn = false;
            }
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public IDeviceState GetState() => null;

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public void RestoreState(IDeviceState state)
        {
        }
    }
}