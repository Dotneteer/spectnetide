using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This block marks the start of a group of blocks which are 
    /// to be treated as one single (composite) block.
    /// </summary>
    public class TzxGroupStartDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Number of group name
        /// </summary>
        public byte Length { get; set; }

        /// <summary>
        /// Group name bytes
        /// </summary>
        public byte[] Chars { get; set; }

        /// <summary>
        /// Gets the group name
        /// </summary>
        public string GroupName => ToAsciiString(Chars);

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x21;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Length = reader.ReadByte();
            Chars = reader.ReadBytes(Length);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Length);
            writer.Write(Chars);
        }
    }
}