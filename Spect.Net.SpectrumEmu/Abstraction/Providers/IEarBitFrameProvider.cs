namespace Spect.Net.SpectrumEmu.Abstraction.Providers
{
    /// <summary>
    /// This interface represents a device that can render beeper pulses
    /// into sound
    /// </summary>
    public interface IEarBitFrameProvider: IVmComponentProvider
    {
        /// <summary>
        /// Adds the specified set of pulse samples to the sound
        /// </summary>
        /// <param name="samples">
        /// Array of sound samples (values between 0.0F and 1.0F)
        /// </param>
        void AddSoundFrame(float[] samples);
    }
}