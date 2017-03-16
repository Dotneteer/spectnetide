using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// Represents the standard speed data block in a TZX file
    /// </summary>
    public class TzxArchiveInfoDataBlock : Tzx3ByteDataBlockBase
    {
        /// <summary>
        /// Length of the whole block (without these two bytes)
        /// </summary>
        public ushort Length { get; set; }

        /// <summary>
        /// Number of text strings
        /// </summary>
        public byte StringCount { get; set; }

        /// <summary>
        /// List of text strings
        /// </summary>
        public TzxText[] TextStrings { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x32;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Length = reader.ReadUInt16();
            StringCount = reader.ReadByte();
            TextStrings = new TzxText[StringCount];
            for (var i = 0; i < StringCount; i++)
            {
                var text = new TzxText();
                text.ReadFrom(reader);
                TextStrings[i] = text;
            }
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Length);
            writer.Write(StringCount);
            foreach (var text in TextStrings)
            {
                text.WriteTo(writer);
            }
        }
    }
}