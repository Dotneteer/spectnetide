using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.DivIde
{
    /// <summary>
    /// This class implements a simnple divIDE device
    /// </summary>
    public class DivIdeDevice: IDivIdeDevice
    {
        private byte _controlReg;

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
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            _controlReg = 0;
        }

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