using Spect.Net.SpectrumEmu.Abstraction;

namespace Spect.Net.SpectrumEmu.Devices.Border
{
    /// <summary>
    /// This class represents the border user by the ULA
    /// </summary>
    public class BorderDevice : IBorderDevice
    {
        private int _borderColor;

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
        /// Gets or sets the ULA border color
        /// </summary>
        public int BorderColor
        {
            get => _borderColor;
            set => _borderColor = value & 0x07;
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            BorderColor = 0;
        }
    }
}