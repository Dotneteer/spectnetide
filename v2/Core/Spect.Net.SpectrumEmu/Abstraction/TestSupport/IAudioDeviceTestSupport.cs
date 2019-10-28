namespace Spect.Net.SpectrumEmu.Abstraction.TestSupport
{
    /// <summary>
    /// This interface defines the operations that support 
    /// the testing of an audio device.
    /// </summary>
    public interface IAudioDeviceTestSupport
    {

        /// <summary>
        /// Index of the next audio sample.
        /// </summary>
        int NextSampleIndex { get; }
    }
}