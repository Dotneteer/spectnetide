using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This is a special block that would normally be generated only by emulators.
    /// </summary>
    public class TzxEmulationInfoDataBlock : TzxDeprecatedDataBlockBase
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x34;

        /// <summary>
        /// Reads through the block infromation, and does not store it
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadThrough(BinaryReader reader)
        {
            reader.ReadBytes(8);
        }
    }
}