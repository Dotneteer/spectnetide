using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This block was created to support the Commodore 64 standard 
    /// ROM and similar tape blocks.
    /// </summary>
    public class TzxC64RomTypeDataBlock : TzxDeprecatedDataBlockBase
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x16;

        /// <summary>
        /// Reads through the block infromation, and does not store it
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadThrough(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            reader.ReadBytes(length - 4);
        }
    }
}