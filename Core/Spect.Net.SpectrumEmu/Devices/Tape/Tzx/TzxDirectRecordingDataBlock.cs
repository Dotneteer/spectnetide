using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// Represents the standard speed data block in a TZX file
    /// </summary>
    public class TzxDirectRecordingDataBlock : Tzx3ByteDataBlockBase
    {
        /// <summary>
        /// Number of T-states per sample (bit of data)
        /// </summary>
        public ushort TactsPerSample { get; set; }

        /// <summary>
        /// Pause after this block
        /// </summary>
        public ushort PauseAfter { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x15;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            TactsPerSample = reader.ReadUInt16();
            PauseAfter = reader.ReadUInt16();
            LastByteUsedBits = reader.ReadByte();
            DataLength = reader.ReadBytes(3);
            Data = reader.ReadBytes(GetLength());
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(TactsPerSample);
            writer.Write(PauseAfter);
            writer.Write(LastByteUsedBits);
            writer.Write(DataLength);
            writer.Write(Data);
        }
    }
}