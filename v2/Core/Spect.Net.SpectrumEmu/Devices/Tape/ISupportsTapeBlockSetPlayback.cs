using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Devices.Tape
{
    /// <summary>
    /// This interface represents that the implementing class supports
    /// emulating tape playback of a set of subsequent tape blocks
    /// </summary>
    public interface ISupportsTapeBlockSetPlayback : ISupportsTapeBlockPlayback
    {
        /// <summary>
        /// Moves the player to the next playable block
        /// </summary>
        /// <param name="currentTact">Tacts time to start the next block</param>
        void NextBlock(long currentTact);
    }
}