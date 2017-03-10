using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This block indicates the end of the Called Sequence.
    /// The next block played will be the block after the last 
    /// CALL block
    /// </summary>
    public class TzxReturnFromSequenceDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x27;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
        }
    }
}