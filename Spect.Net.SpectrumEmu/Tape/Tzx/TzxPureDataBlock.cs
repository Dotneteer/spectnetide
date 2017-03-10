using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Represents the standard speed data block in a TZX file
    /// </summary>
    public class TzxPureDataBlock : Tzx3ByteDataBlockBase
    {
        /// <summary>
        /// Length of the zero bit
        /// </summary>
        public ushort ZeroBitPulseLength { get; set; }

        /// <summary>
        /// Length of the one bit
        /// </summary>
        public ushort OneBitPulseLength { get; set; }

        /// <summary>
        /// Pause after this block
        /// </summary>
        public ushort PauseAfter { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x14;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            ZeroBitPulseLength = reader.ReadUInt16();
            OneBitPulseLength = reader.ReadUInt16();
            LastByteUsedBits = reader.ReadByte();
            PauseAfter = reader.ReadUInt16();
            DataLength = reader.ReadBytes(3);
            Data = reader.ReadBytes(GetLength());
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(ZeroBitPulseLength);
            writer.Write(OneBitPulseLength);
            writer.Write(LastByteUsedBits);
            writer.Write(PauseAfter);
            writer.Write(DataLength);
            writer.Write(Data);
        }
    }
}