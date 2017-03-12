using System;
using System.Collections.Generic;
using System.IO;
using Spect.Net.SpectrumEmu.Tape.Tzx;

namespace Spect.Net.SpectrumEmu.Tape
{
    /// <summary>
    /// This class is responsible to "play" a TZX file.
    /// </summary>
    public class TzxPlayer : ISupportsTapePlayback
    {
        /// <summary>
        /// Available TZX data block types and types handling them
        /// </summary>
        public static Dictionary<byte, Type> DataBlockTypes = new Dictionary<byte, Type>
        {
            {0x10, typeof(TzxStandardSpeedDataBlock)},
            {0x11, typeof(TzxTurboSpeedDataBlock)},
            {0x12, typeof(TzxPureToneDataBlock)},
            {0x13, typeof(TzxPulseSequenceDataBlock)},
            {0x14, typeof(TzxPureDataBlock)},
            {0x15, typeof(TzxDirectRecordingDataBlock)},
            {0x16, typeof(TzxC64RomTypeDataBlock)},
            {0x17, typeof(TzxC64TurboTapeDataBlock)},
            {0x18, typeof(TzxCswRecordingDataBlock)},
            {0x19, typeof(TzxGeneralizedDataBlock)},
            {0x20, typeof(TzxSilenceDataBlock)},
            {0x21, typeof(TzxGroupStartDataBlock)},
            {0x22, typeof(TzxGroupEndDataBlock)},
            {0x23, typeof(TzxJumpDataBlock)},
            {0x24, typeof(TzxLoopStartDataBlock)},
            {0x25, typeof(TzxLoopEndDataBlock)},
            {0x26, typeof(TzxCallSequenceDataBlock)},
            {0x27, typeof(TzxReturnFromSequenceDataBlock)},
            {0x28, typeof(TzxSelectDataBlock)},
            {0x2A, typeof(TzxStopTheTape48DataBlock)},
            {0x2B, typeof(TzxSetSignalLevelDataBlock)},
            {0x30, typeof(TzxTextDescriptionDataBlock)},
            {0x31, typeof(TzxMessageDataBlock)},
            {0x32, typeof(TzxArchiveInfoDataBlock)},
            {0x33, typeof(TzxHardwareInfoDataBlock)},
            {0x34, typeof(TzxEmulationInfoDataBlock)},
            {0x35, typeof(TzxCustomInfoDataBlock)},
            {0x40, typeof(TzxSnapshotBlock)},
            {0x5A, typeof(TzxGlueDataBlock)},
        };


        private readonly BinaryReader _reader;

        /// <summary>
        /// Data blocks of this TZX file
        /// </summary>
        public IList<TzxDataBlockBase> DataBlocks { get; }

        /// <summary>
        /// Major version number of the file
        /// </summary>
        public byte MajorVersion { get; private set; }

        /// <summary>
        /// Minor version number of the file
        /// </summary>
        public byte MinorVersion { get; private set; }

        /// <summary>
        /// Initializes the player from the specified reader
        /// </summary>
        /// <param name="reader"></param>
        public TzxPlayer(BinaryReader reader)
        {
            _reader = reader;
            DataBlocks = new List<TzxDataBlockBase>();
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
        public ulong StartTact { get; private set; }

        /// <summary>
        /// Reads in the content of the TZX file so that it can be played
        /// </summary>
        public void ReadContent()
        {
            var header = new TzxHeader();
            header.ReadFrom(_reader);
            if (!header.IsValid)
            {
                throw new TzxException("Invalid TZX header");
            }
            MajorVersion = header.MajorVersion;
            MinorVersion = header.MinorVersion;

            while (_reader.BaseStream.Position != _reader.BaseStream.Length)
            {
                var blockType = _reader.ReadByte();
                if (!DataBlockTypes.TryGetValue(blockType, out var type))
                {
                    throw new TzxException($"Unkonwn TZX block type: {blockType}");
                }

                try
                {
                    var block = Activator.CreateInstance(type) as TzxDataBlockBase;
                    if (block is TzxDeprecatedDataBlockBase deprecated)
                    {
                        deprecated.ReadThrough(_reader);
                    }
                    else
                    {
                        block?.ReadFrom(_reader);
                    }
                    DataBlocks.Add(block);
                }
                catch (Exception ex)
                {
                    throw new TzxException($"Cannot read TZX data block {type}.", ex);
                }
            }
        }

        /// <summary>
        /// Initializes the player
        /// </summary>
        public void InitPlay(ulong startTact)
        {
            CurrentBlockIndex = -1;
            JumpToNextPlayableBlock(startTact);
            PlayPhase = PlayPhase.None;
            StartTact = startTact;
        }

        /// <summary>
        /// Gets the EAR bit value for the specified tact
        /// </summary>
        /// <param name="currentTact">Tact to retrieve the EAR bit</param>
        /// <returns>
        /// A tuple of the EAR bit and a flag that indicates it is time to move to the next block
        /// </returns>
        public bool GetEarBit(ulong currentTact)
        {
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
        /// <param name="currentTact">Tact time to start the next block</param>
        private void JumpToNextPlayableBlock(ulong currentTact)
        {
            while (++CurrentBlockIndex < DataBlocks.Count && !DataBlocks[CurrentBlockIndex].SupportPlayback) { }
            CurrentBlock = DataBlocks[CurrentBlockIndex] as ISupportsTapePlayback;
            CurrentBlock?.InitPlay(currentTact);
        }
    }
}