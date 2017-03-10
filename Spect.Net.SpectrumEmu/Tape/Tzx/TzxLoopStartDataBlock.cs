using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// If you have a sequence of identical blocks, or of identical 
    /// groups of blocks, you can use this block to tell how many 
    /// times they should be repeated.
    /// </summary>
    public class TzxLoopStartDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Number of repetitions (greater than 1)
        /// </summary>
        public ushort Loops { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x24;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Loops = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Loops);
        }
    }
}