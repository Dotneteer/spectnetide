using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This is meant to identify parts of the tape, so you know where level 1 starts, 
    /// where to rewind to when the game ends, etc.
    /// </summary>
    /// <remarks>
    /// This description is not guaranteed to be shown while the tape is playing, 
    /// but can be read while browsing the tape or changing the tape pointer.
    /// </remarks>
    public class TzxTextDescriptionDataBlock: TzxDataBlockBase
    {
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
        public string DescriptionText => ToAsciiString(Description);

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x30;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            DescriptionLength = reader.ReadByte();
            Description = reader.ReadBytes(DescriptionLength);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(DescriptionLength);
            writer.Write(Description);
        }
    }
}