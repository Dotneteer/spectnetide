using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This block is generated when you merge two ZX Tape files together.
    /// </summary>
    /// <remarks>
    /// It is here so that you can easily copy the files together and use 
    /// them. Of course, this means that resulting file would be 10 bytes 
    /// longer than if this block was not used. All you have to do if 
    /// you encounter this block ID is to skip next 9 bytes.
    /// </remarks>
    public class TzxGlueDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Signs that this block is not playable
        /// </summary>
        public override bool IsPlayable => false;

        /// <summary>
        /// Value: { "XTape!", 0x1A, MajorVersion, MinorVersion }
        /// </summary>
        /// <remarks>
        /// Just skip these 9 bytes and you will end up on the next ID.
        /// </remarks>
        public byte[] Glue { get; set; }

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x5A;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Glue = reader.ReadBytes(9);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Glue);
        }
    }
}