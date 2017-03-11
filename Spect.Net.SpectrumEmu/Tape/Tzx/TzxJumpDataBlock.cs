using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This block will enable you to jump from one block to another within the file.
    /// </summary>
    /// <remarks>
    /// Jump 0 = 'Loop Forever' - this should never happen
    /// Jump 1 = 'Go to the next block' - it is like NOP in assembler
    /// Jump 2 = 'Skip one block'
    /// Jump -1 = 'Go to the previous block'
    /// </remarks>
    public class TzxJumpDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Signs that this block is not playable
        /// </summary>
        public override bool IsPlayable => false;

        /// <summary>
        /// Relative jump value
        /// </summary>
        /// <remarks>
        /// </remarks>
        public short Jump { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x23;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Jump = reader.ReadInt16();
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Jump);
        }
    }
}