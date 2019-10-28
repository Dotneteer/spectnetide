using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Providers
{
    /// <summary>
    /// Default audio provider used by the scripting engine.
    /// </summary>
    /// <remarks>
    /// This audio provider does not provide any sound.
    /// </remarks>
    public class NoAudioProvider : ISoundProvider
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
        public ISpectrumVm HostVm { get; set; }

        /// <summary>
        /// Signs that the provider has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// Adds the specified set of pulse samples to the sound
        /// </summary>
        /// <param name="samples">
        /// Array of sound samples (values between 0.0F and 1.0F)
        /// </param>
        public void AddSoundFrame(float[] samples)
        {
        }

        /// <summary>
        /// Starts playing the sound
        /// </summary>
        public void PlaySound()
        {
        }

        /// <summary>
        /// Pauses playing the sound
        /// </summary>
        public void PauseSound()
        {
        }

        /// <summary>
        /// Stops playing the sound
        /// </summary>
        public void KillSound()
        {
        }
    }
}