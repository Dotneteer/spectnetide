using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This class is responsible to "play" a TZX file.
    /// </summary>
    public class TzxPlayer : TzxReader, ISupportsTapePlayback
    {
        private int _lastPlayableIndex;

        /// <summary>
        /// Signs that the player completed playing back the file
        /// </summary>
        public bool Eof { get; private set; }

        /// <summary>
        /// Initializes the player from the specified reader
        /// </summary>
        /// <param name="reader">BinaryReader instance to get TZX file data from</param>
        public TzxPlayer(BinaryReader reader): base(reader)
        {
        }

        /// <summary>
        /// Gets the currently playing block's index
        /// </summary>
        public int CurrentBlockIndex { get; private set; }

        /// <summary>
        /// The current playable block
        /// </summary>
        public ISupportsTapePlayback CurrentBlock { get; private set; }

        /// <summary>
        /// The current playing phase
        /// </summary>
        public PlayPhase PlayPhase { get; private set; }

        /// <summary>
        /// The tact count of the CPU when playing starts
        /// </summary>
        public long StartTact { get; private set; }

        /// <summary>
        /// Reads in the content of the TZX file so that it can be played
        /// </summary>
        public override bool ReadContent()
        {
            if (base.ReadContent())
            {
                // --- Precalculate info for EOF check
                _lastPlayableIndex = -1;
                for (var i = DataBlocks.Count - 1; i >= 0; i--)
                {
                    if ((DataBlocks[i] as ISupportsTapePlayback) == null) continue;

                    _lastPlayableIndex = i;
                    break;
                }
                Eof = false;
            }
            else
            {
                Eof = true;
            }
            return !Eof;
        }

        /// <summary>
        /// Initializes the player
        /// </summary>
        public void InitPlay(long startTact)
        {
            CurrentBlockIndex = -1;
            JumpToNextPlayableBlock(startTact);
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
            if (CurrentBlockIndex == _lastPlayableIndex 
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
                JumpToNextPlayableBlock(currentTact);
            }
            return earbit;
        }

        /// <summary>
        /// Moves the current block index to the next playable block
        /// </summary>
        /// <param name="currentTact">Tacts time to start the next block</param>
        public void JumpToNextPlayableBlock(long currentTact)
        {
            while (++CurrentBlockIndex < DataBlocks.Count && !DataBlocks[CurrentBlockIndex].SupportPlayback) { }
            if (CurrentBlockIndex >= DataBlocks.Count)
            {
                PlayPhase = PlayPhase.Completed;
                return;
            }
            CurrentBlock = DataBlocks[CurrentBlockIndex] as ISupportsTapePlayback;
            CurrentBlock?.InitPlay(currentTact);
        }
    }
}