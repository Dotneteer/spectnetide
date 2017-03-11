using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This block is an analogue of the CALL Subroutine statement.
    /// </summary>
    /// <remarks>
    /// It basically executes a sequence of blocks that are somewhere 
    /// else and then goes back to the next block. Because more than 
    /// one call can be normally used you can include a list of sequences 
    /// to be called. The 'nesting' of call blocks is also not allowed 
    /// for the simplicity reasons. You can, of course, use the CALL 
    /// blocks in the LOOP sequences and vice versa.
    /// </remarks>
    public class TzxCallSequenceDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Signs that this block is not playable
        /// </summary>
        public override bool IsPlayable => false;

        /// <summary>
        /// Number of group name
        /// </summary>
        public byte NumberOfCalls { get; set; }

        /// <summary>
        /// Group name bytes
        /// </summary>
        public ushort[] BlockOffsets { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x26;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            NumberOfCalls = reader.ReadByte();
            BlockOffsets = ReadWords(reader, NumberOfCalls);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(NumberOfCalls);
            WriteWords(writer, BlockOffsets);
        }
    }
}