using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// Represents the standard speed data block in a TZX file
    /// </summary>
    public class TzxStandardSpeedDataBlock : TzxDataBlockBase, ISupportsTapeBlockPlayback, ITapeData
    {
        private TapeDataBlockPlayer _player;

        /// <summary>
        /// Pause after this block (default: 1000ms)
        /// </summary>
        public ushort PauseAfter { get; set; } = 1000;

        /// <summary>
        /// Lenght of block data
        /// </summary>
        public ushort DataLength { get; set; }

        /// <summary>
        /// Block Data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x10;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            PauseAfter = reader.ReadUInt16();
            DataLength = reader.ReadUInt16();
            Data = reader.ReadBytes(DataLength);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)BlockId);
            writer.Write(PauseAfter);
            writer.Write(DataLength);
            writer.Write(Data, 0, DataLength);
        }

        /// <summary>
        /// The index of the currently playing byte
        /// </summary>
        /// <remarks>This proprty is made public for test purposes</remarks>
        public int ByteIndex => _player.ByteIndex;

        /// <summary>
        /// The mask of the currently playing bit in the current byte
        /// </summary>
        public byte BitMask => _player.BitMask;

        /// <summary>
        /// The current playing phase
        /// </summary>
        public PlayPhase PlayPhase => _player.PlayPhase;

        /// <summary>
        /// The tact count of the CPU when playing starts
        /// </summary>
        public long StartTact => _player.StartTact;

        /// <summary>
        /// Last tact queried
        /// </summary>
        public long LastTact => _player.LastTact;

        /// <summary>
        /// Initializes the player
        /// </summary>
        public void InitPlay(long startTact)
        {
            _player = new TapeDataBlockPlayer(Data, PauseAfter);
            _player.InitPlay(startTact);
        }

        /// <summary>
        /// Gets the EAR bit value for the specified tact
        /// </summary>
        /// <param name="currentTact">Tacts to retrieve the EAR bit</param>
        /// <returns>
        /// The EAR bit value to play back
        /// </returns>
        public bool GetEarBit(long currentTact) => _player.GetEarBit(currentTact);
    }
}