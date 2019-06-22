using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This block was created to support the Commodore 64 standard 
    /// ROM and similar tape blocks.
    /// </summary>
    public class TzxSnapshotBlock : TzxDeprecatedDataBlockBase
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x40;

        /// <summary>
        /// Reads through the block infromation, and does not store it
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadThrough(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            length = length & 0x00FFFFFF;
            reader.ReadBytes(length);
        }
    }
}