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

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x19;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            BlockLength = reader.ReadUInt32();
            PauseAfter = reader.ReadUInt16();
            Totp = reader.ReadUInt32();
            Npp = reader.ReadByte();
            Asp = reader.ReadByte();
            Totd = reader.ReadUInt32();
            Npd = reader.ReadByte();
            Asd = reader.ReadByte();

            PilotSymDef = new TzxSymDef[Asp];
            for (var i = 0; i < Asp; i++)
            {
                var symDef = new TzxSymDef(Npp);
                symDef.ReadFrom(reader);
                PilotSymDef[i] = symDef;
            }

            PilotStream = new TzxPrle[Totp];
            for (var i = 0; i < Totp; i++)
            {
                PilotStream[i].Symbol = reader.ReadByte();
                PilotStream[i].Repetitions = reader.ReadUInt16();
            }

            DataSymDef = new TzxSymDef[Asd];
            for (var i = 0; i < Asd; i++)
            {
                var symDef = new TzxSymDef(Npd);
                symDef.ReadFrom(reader);
                DataSymDef[i] = symDef;
            }

            DataStream = new TzxPrle[Totd];
            for (var i = 0; i < Totd; i++)
            {
                DataStream[i].Symbol = reader.ReadByte();
                DataStream[i].Repetitions = reader.ReadUInt16();
            }
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(BlockLength);
            writer.Write(PauseAfter);
            writer.Write(Totp);
            writer.Write(Npp);
            writer.Write(Asp);
            writer.Write(Totd);
            writer.Write(Npd);
            writer.Write(Asd);
            for (var i = 0; i < Asp; i++)
            {
                PilotSymDef[i].WriteTo(writer);
            }
            for (var i = 0; i < Totp; i++)
            {
                writer.Write(PilotStream[i].Symbol);
                writer.Write(PilotStream[i].Repetitions);
            }

            for (var i = 0; i < Asd; i++)
            {
                DataSymDef[i].WriteTo(writer);
            }

            for (var i = 0; i < Totd; i++)
            {
                writer.Write(DataStream[i].Symbol);
                writer.Write(DataStream[i].Repetitions);
            }
        }
    }
}