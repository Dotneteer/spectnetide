using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This block represents select structure
    /// </summary>
    public class TzxSelect: ITzxSerialization
    {
        /// <summary>
        /// Bit 0 - Bit 1: Starting symbol polarity
        /// </summary>
        /// <remarks>
        /// 00: opposite to the current level (make an edge, as usual) - default
        /// 01: same as the current level(no edge - prolongs the previous pulse)
        /// 10: force low level
        /// 11: force high level
        /// </remarks>
        public ushort BlockOffset;

        /// <summary>
        /// Length of the description
        /// </summary>
        public byte DescriptionLength { get; set; }

        /// <summary>
        /// The description bytes
        /// </summary>
        public byte[] Description;

        /// <summary>
        /// The string form of description
        /// </summary>
        public string DescriptionText => TzxDataBlockBase.ToAsciiString(Description);

        public TzxSelect(byte length)
        {
            DescriptionLength = length;
        }

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public void ReadFrom(BinaryReader reader)
        {
            BlockOffset = reader.ReadUInt16();
            DescriptionLength = reader.ReadByte();
            Description = reader.ReadBytes(DescriptionLength);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(BlockOffset);
            writer.Write(DescriptionLength);
            writer.Write(Description);
        }
    }
}