using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Represents a generalized data block in a TZX file
    /// </summary>
    public class TzxGeneralizedDataBlock : TzxDataBlockBase
    {
        /// <summary>
        /// Block length (without these four bytes)
        /// </summary>
        public uint BlockLength { get; set; }

        /// <summary>
        /// Pause after this block 
        /// </summary>
        public ushort PauseAfter { get; set; }

        /// <summary>
        /// Total number of symbols in pilot/sync block (can be 0)
        /// </summary>
        public uint Totp { get; set; }

        /// <summary>
        /// Maximum number of pulses per pilot/sync symbol
        /// </summary>
        public byte Npp { get; set; }

        /// <summary>
        /// Number of pilot/sync symbols in the alphabet table (0=256)
        /// </summary>
        public byte Asp { get; set; }

        /// <summary>
        /// Total number of symbols in data stream (can be 0)
        /// </summary>
        public uint Totd { get; set; }

        /// <summary>
        /// Maximum number of pulses per data symbol
        /// </summary>
        public byte Npd { get; set; }

        /// <summary>
        /// Number of data symbols in the alphabet table (0=256)
        /// </summary>
        public byte Asd { get; set; }

        /// <summary>
        /// Pilot and sync symbols definition table
        /// </summary>
        /// <remarks>
        /// This field is present only if Totp > 0
        /// </remarks>
        public TzxSymDef[] PilotSymDef { get; set; }

        /// <summary>
        /// Pilot and sync data stream
        /// </summary>
        /// <remarks>
        /// This field is present only if Totd > 0
        /// </remarks>
        public TzxPrle[] PilotStream { get; set; }

        /// <summary>
        /// Data symbols definition table
        /// </summary>
        /// <remarks>
        /// This field is present only if Totp > 0
        /// </remarks>
        public TzxSymDef[] DataSymDef { get; set; }

        /// <summary>
        /// Data stream
        /// </summary>
        /// <remarks>
        /// This field is present only if Totd > 0
        /// </remarks>
        public TzxPrle[] DataStream { get; set; }

        #region Overrides of TzxDataBlockBase

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x13;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            // TODO: Implement this method
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            // TODO: Implement this method
        }

        #endregion
    }
}