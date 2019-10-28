namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Tape
{
    /// <summary>
    /// Represents the playing phase of the current block.
    /// </summary>
    public enum PlayPhase
    {
        /// <summary>
        /// The player is passive.
        /// </summary>
        None = 0,

        /// <summary>
        /// Pilot signals.
        /// </summary>
        Pilot,

        /// <summary>
        /// Sync signals at the end of the pilot.
        /// </summary>
        Sync,

        /// <summary>
        /// Bits in the data block.
        /// </summary>
        Data,

        /// <summary>
        /// Short terminating sync signal before pause.
        /// </summary>
        TermSync,

        /// <summary>
        /// Pause after the data block.
        /// </summary>
        Pause,

        /// <summary>
        /// The entire block has been played back.
        /// </summary>
        Completed
    }
}