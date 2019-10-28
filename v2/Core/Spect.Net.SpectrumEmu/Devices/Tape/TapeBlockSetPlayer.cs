using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Devices.Tape
{
    /// <summary>
    /// This class is responsible to "play" a TZX file.
    /// </summary>
    public class TapeBlockSetPlayer : ISupportsTapeBlockSetPlayback
    {
        /// <summary>
        /// All data blocks that can be played back
        /// </summary>
        public List<ISupportsTapeBlockPlayback> DataBlocks { get; }

        /// <summary>
        /// Signs that the player completed playing back the file
        /// </summary>
        public bool Eof { get; private set; }

        /// <summary>
        /// Gets the currently playing block's index
        /// </summary>
        public int CurrentBlockIndex { get; private set; }

        /// <summary>
        /// The current playable block
        /// </summary>
        public ISupportsTapeBlockPlayback CurrentBlock => DataBlocks[CurrentBlockIndex];

        /// <summary>
        /// The current playing phase
        /// </summary>
        public PlayPhase PlayPhase { get; private set; }

        /// <summary>
        /// The tact count of the CPU when playing starts
        /// </summary>
        public long StartTact { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TapeBlockSetPlayer(List<ISupportsTapeBlockPlayback> dataBlocks)
        {
            DataBlocks = dataBlocks;
            Eof = dataBlocks.Count == 0;
        }

        /// <summary>
        /// Initializes the player
        /// </summary>
        public void InitPlay(long startTact)
        {
            CurrentBlockIndex = -1;
            NextBlock(startTact);
            PlayPhase = PlayPhase.None;
            StartTact = startTact;
        }

        /// <summary>
        /// Gets the EAR bit value for the specified tact
        /// </summary>
        /// <param name="currentTact">Tacts to retrieve the EAR bit</param>
        /// <returns>
        /// A tuple of the EAR bit and a flag that indicates it is time to move to the next block
        /// </returns>
        public bool GetEarBit(long currentTact)
        {
            // --- Check for EOF
            if (CurrentBlockIndex == DataBlocks.Count - 1 
                && (CurrentBlock.PlayPhase == PlayPhase.Pause || CurrentBlock.PlayPhase == PlayPhase.Completed))
            {
                Eof = true;
            }
            if (CurrentBlockIndex >= DataBlocks.Count || CurrentBlock == null)
            {
                // --- After all playable block played back, there's nothing more to do
                PlayPhase = PlayPhase.Completed;
                return true;
            }
            var earbit = CurrentBlock.GetEarBit(currentTact);
            if (CurrentBlock.PlayPhase == PlayPhase.Completed)
            {
                NextBlock(currentTact);
            }
            return earbit;
        }

        /// <summary>
        /// Moves the current block index to the next playable block
        /// </summary>
        /// <param name="currentTact">Tacts time to start the next block</param>
        public void NextBlock(long currentTact)
        {
            if (CurrentBlockIndex >= DataBlocks.Count - 1)
            {
                PlayPhase = PlayPhase.Completed;
                Eof = true;
                return;
            }
            CurrentBlockIndex++;
            CurrentBlock.InitPlay(currentTact);
        }
    }
}