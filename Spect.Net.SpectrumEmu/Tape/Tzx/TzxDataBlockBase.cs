using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This class describes a TZX Block
    /// </summary>
    public abstract class TzxDataBlockBase
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public abstract int BlockId { get; }

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public abstract void ReadFrom(BinaryReader reader);

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public abstract void WriteTo(BinaryWriter writer);

        /// <summary>
        /// Override this method to check the content of the block
        /// </summary>
        public virtual bool IsValid => true;
    }
}