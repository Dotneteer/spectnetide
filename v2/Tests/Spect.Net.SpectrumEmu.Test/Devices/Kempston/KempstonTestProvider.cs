using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Kempston
{
    /// <summary>
    /// This class implements a Kempston provider for automatic tests
    /// </summary>
    public class KempstonTestProvider : IKempstonProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// The virtual machine that hosts the provider
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the provider has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// Indicates if the Kempston device is present.
        /// </summary>
        public bool IsPresent { get; set; }

        /// <summary>
        /// The flag that indicates if the left button is pressed.
        /// </summary>
        public bool LeftPressed { get; set; }

        /// <summary>
        /// The flag that indicates if the right button is pressed.
        /// </summary>
        public bool RightPressed { get; set; }

        /// <summary>
        /// The flag that indicates if the up button is pressed.
        /// </summary>
        public bool UpPressed { get; set; }

        /// <summary>
        /// The flag that indicates if the down button is pressed.
        /// </summary>
        public bool DownPressed { get; set; }

        /// <summary>
        /// The flag that indicates if the fire button is pressed.
        /// </summary>
        public bool FirePressed { get; set; }
    }
}