namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Base class for all TZX block type with data length of 3 bytes
    /// </summary>
    public abstract class Tzx3ByteDataBlockBase : TzxDataBlockBase
    {
        /// <summary>
        /// Used bits in the last byte (other bits should be 0)
        /// </summary>
        /// <remarks>
        /// (e.g. if this is 6, then the bits used(x) in the last byte are: 
        /// xxxxxx00, where MSb is the leftmost bit, LSb is the rightmost bit)
        /// </remarks>
        public byte LastByteUsedBits { get; set; }

        /// <summary>
        /// Lenght of block data
        /// </summary>
        public byte[] DataLength { get; set; }

        /// <summary>
        /// Block Data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Override this method to check the content of the block
        /// </summary>
        public override bool IsValid => GetLength() == Data.Length;

        /// <summary>
        /// Calculates data length
        /// </summary>
        protected int GetLength()
        {
            return DataLength[0] + DataLength[1] << 8 + DataLength[2] << 16;
        }
    }
}