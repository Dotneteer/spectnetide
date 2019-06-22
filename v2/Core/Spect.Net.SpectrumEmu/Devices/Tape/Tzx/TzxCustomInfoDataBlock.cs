using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// Represents the standard speed data block in a TZX file
    /// </summary>
    public class TzxCustomInfoDataBlock : Tzx3ByteDataBlockBase
    {
        /// <summary>
        /// Identification string (in ASCII)
        /// </summary>
        public byte[] Id { get; set; }

        /// <summary>
        /// String representation of the ID
        /// </summary>
        public string IdText => ToAsciiString(Id);

        /// <summary>
        /// Length of the custom info
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// Custom information
        /// </summary>
        public byte[] CustomInfo { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x35;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Id = reader.ReadBytes(10);
            Length = reader.ReadUInt32();
            CustomInfo = reader.ReadBytes((int)Length);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Length);
            writer.Write(CustomInfo);
        }
    }
}