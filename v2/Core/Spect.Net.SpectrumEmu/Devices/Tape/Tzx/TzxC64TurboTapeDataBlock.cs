using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This block is made to support another type of encoding that is 
    /// commonly used by the C64.
    /// </summary>
    public class TzxC64TurboTapeDataBlock : TzxDeprecatedDataBlockBase
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x17;

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