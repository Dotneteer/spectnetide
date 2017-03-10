using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Represents the standard speed data block in a TZX file
    /// </summary>
    public class TzxPureToneDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Pause after this block
        /// </summary>
        public ushort PulseLength { get; private set; }

        /// <summary>
        /// Lenght of block data
        /// </summary>
        public ushort PulseCount { get; private set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x12;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            PulseLength = reader.ReadUInt16();
            PulseCount = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(PulseLength);
            writer.Write(PulseCount);
        }
    }
}