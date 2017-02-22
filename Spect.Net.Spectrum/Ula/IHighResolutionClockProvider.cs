namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This interface defines the behavior of a high resolution clock provider
    /// that can be used for timing tasks.
    /// </summary>
    public interface IHighResolutionClockProvider
    {
        /// <summary>
        /// Retrieves the frequency of the clock. This value shows new
        /// number of clock ticks per second.
        /// </summary>
        long GetFrequency();

        /// <summary>
        /// Retrieves the current counter value of the clock.
        /// </summary>
        long GetCounter();
    }
}