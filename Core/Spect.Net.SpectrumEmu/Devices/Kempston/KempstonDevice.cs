using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Devices.Kempston
{
    /// <summary>
    /// This class represents the Kempston joystick device
    /// </summary>
    public class KempstonDevice: IKempstonDevice
    {
        private IKempstonProvider _kempstonProvider;

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        IDeviceState IDevice.GetState() => null;

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public void RestoreState(IDeviceState state)
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
            var kempstonInfo = HostVm.GetDeviceInfo<IKempstonDevice>();
            _kempstonProvider = (IKempstonProvider)kempstonInfo?.Provider;
        }

        /// <summary>
        /// Indicates if the Kempston device is present.
        /// </summary>
        public bool IsPresent => _kempstonProvider?.IsPresent ?? false;

        /// <summary>
        /// The flag that indicates if the left button is pressed.
        /// </summary>
        public bool LeftPressed => _kempstonProvider?.LeftPressed ?? false;

        /// <summary>
        /// The flag that indicates if the right button is pressed.
        /// </summary>
        public bool RightPressed => _kempstonProvider?.RightPressed ?? false;

        /// <summary>
        /// The flag that indicates if the up button is pressed.
        /// </summary>
        public bool UpPressed => _kempstonProvider?.UpPressed ?? false;

        /// <summary>
        /// The flag that indicates if the down button is pressed.
        /// </summary>
        public bool DownPressed => _kempstonProvider?.DownPressed ?? false;

        /// <summary>
        /// The flag that indicates if the fire button is pressed.
        /// </summary>
        public bool FirePressed => _kempstonProvider?.FirePressed ?? false;
    }
}