namespace Spect.Net.SpectrumEmu.Devices.Next
{
    /// <summary>
    /// This class represents the Turbo Control Register (0x07)
    /// </summary>
    public class TurboControlRegister : FeatureControlRegisterBase
    {
        /// <summary>
        /// Initializes a new instance of the feature control register.
        /// </summary>
        /// <remarks>
        /// Sets the Turbo speed value
        /// </remarks>
        public TurboControlRegister() : base(0x40, false)
        {
        }
    }
}