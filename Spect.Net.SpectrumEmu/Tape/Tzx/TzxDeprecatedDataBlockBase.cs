using System;
using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// This class represents a deprecated block
    /// </summary>
    public abstract class TzxDeprecatedDataBlockBase : TzxDataBlockBase
    {
        /// <summary>
        /// Signs that this block is not playable
        /// </summary>
        public override bool IsPlayable => false;

        /// <summary>
        /// Reads through the block infromation, and does not store it
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public abstract void ReadThrough(BinaryReader reader);

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
            throw new InvalidOperationException("Deprecated TZX data blocks cannot be written.");
        }
    }
}