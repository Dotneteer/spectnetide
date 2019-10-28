using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Abstraction.TestSupport
{
    /// <summary>
    /// This interface represents a testable tape player.
    /// </summary>
    public interface ITestablePlayer
    {
        /// <summary>
        /// The current playing phase.
        /// </summary>
        PlayPhase PlayPhase { get; }

        /// <summary>
        /// The current playable block.
        /// </summary>
        ISupportsTapeBlockPlayback CurrentBlock { get; }

        /// <summary>
        /// Moves the current block index to the next playable block.
        /// </summary>
        /// <param name="currentTact">Tacts time to start the next block.</param>
        void NextBlock(long currentTact);
    }
}