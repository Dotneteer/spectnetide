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
    public class TzxText: ITzxSerialization
    {
        /// <summary>
        /// Text identification byte.
        /// </summary>
        /// <remarks>
        /// 00 - Full title
        /// 01 - Software house/publisher
        /// 02 - Author(s)
        /// 03 - Year of publication
        /// 04 - Language
        /// 05 - Game/utility type
        /// 06 - Price
        /// 07 - Protection scheme/loader
        /// 08 - Origin
        /// FF - Comment(s)
        /// </remarks>
        public byte Type { get; set; }

        /// <summary>
        /// Length of the description
        /// </summary>
        public byte Length { get; set; }

        /// <summary>
        /// The description bytes
        /// </summary>
        public byte[] TextBytes;

        /// <summary>
        /// The string form of description
        /// </summary>
        public string Text => TzxDataBlockBase.ToAsciiString(TextBytes);

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public void ReadFrom(BinaryReader reader)
        {
            Length = reader.ReadByte();
            TextBytes = reader.ReadBytes(Length);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Length);
            writer.Write(TextBytes);
        }
    }
}