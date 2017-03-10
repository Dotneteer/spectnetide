using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Pause (silence) or 'Stop the Tape' block
    /// </summary>
    public class TzxSelectDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Length of the whole block (without these two bytes)
        /// </summary>
        public ushort Length { get; set; }

        /// <summary>
        /// Number of selections
        /// </summary>
        public byte SelectionCount { get; set; }

        /// <summary>
        /// List of selections
        /// </summary>
        public TzxSelect[] Selections { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x28;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Length = reader.ReadUInt16();
            SelectionCount = reader.ReadByte();
            Selections = new TzxSelect[SelectionCount];
            foreach (var selection in Selections)
            {
                selection.ReadFrom(reader);
            }
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Length);
            writer.Write(SelectionCount);
            foreach (var selection in Selections)
            {
                selection.WriteTo(writer);
            }
        }
    }
}