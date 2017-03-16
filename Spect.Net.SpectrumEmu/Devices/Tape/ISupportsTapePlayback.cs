namespace Spect.Net.SpectrumEmu.Devices.Tape
{
    /// <summary>
    /// This interface represents that the implementing class supports
    /// emulating tape playback
    /// </summary>
    public interface ISupportsTapePlayback
    {
        /// <summary>
        /// The current playing phase
        /// </summary>
        PlayPhase PlayPhase { get; }

        /// <summary>
        /// The tact count of the CPU when playing starts
        /// </summary>
        ulong StartTact { get; }

        /// <summary>
        /// Initializes the player
        /// </summary>
        void InitPlay(ulong startTact);

        /// <summary>
        /// Gets the EAR bit value for the specified tact
        /// </summary>
        /// <param name="currentTact">Tact to retrieve the EAR bit</param>
        /// <returns>
        /// The EAR bit value to play back
        /// </returns>
        bool GetEarBit(ulong currentTact);
    }
}