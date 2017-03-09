using System.IO;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Represents the standard speed data block in a TZX file
    /// </summary>
    public class TzxTurboSpeedDataBlock : Tzx3ByteDataBlockBase
    {
        /// <summary>
        /// Length of pilot pulse
        /// </summary>
        public ushort PilotPulseLength { get; set; }

        /// <summary>
        /// Length of the first sync pulse
        /// </summary>
        public ushort Sync1PulseLength { get; set; }

        /// <summary>
        /// Length of the second sync pulse
        /// </summary>
        public ushort Sync2PulseLength { get; set; }

        /// <summary>
        /// Length of the zero bit
        /// </summary>
        public ushort ZeroBitPulseLength { get; set; }

        /// <summary>
        /// Length of the one bit
        /// </summary>
        public ushort OneBitPulseLength { get; set; }

        /// <summary>
        /// Length of the pilot tone
        /// </summary>
        public ushort PilotToneLength { get; set; }

        /// <summary>
        /// Pause after this block
        /// </summary>
        public ushort PauseAfter { get; set; }

        public TzxTurboSpeedDataBlock()
        {
            PilotPulseLength = 2168;
            Sync1PulseLength = 667;
            Sync2PulseLength = 735;
            ZeroBitPulseLength = 855;
            OneBitPulseLength = 1710;
            PilotToneLength = 8063;
            LastByteUsedBits = 8;
        }

        #region Overrides of TzxDataBlockBase

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x11;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            PilotPulseLength = reader.ReadUInt16();
            Sync1PulseLength = reader.ReadUInt16();
            Sync2PulseLength = reader.ReadUInt16();
            ZeroBitPulseLength = reader.ReadUInt16();
            OneBitPulseLength = reader.ReadUInt16();
            PilotToneLength = reader.ReadUInt16();
            LastByteUsedBits = reader.ReadByte();
            PauseAfter = reader.ReadUInt16();
            DataLength = reader.ReadBytes(3);
            Data = reader.ReadBytes(GetLength());
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(PilotPulseLength);
            writer.Write(Sync1PulseLength);
            writer.Write(Sync2PulseLength);
            writer.Write(ZeroBitPulseLength);
            writer.Write(OneBitPulseLength);
            writer.Write(PilotToneLength);
            writer.Write(LastByteUsedBits);
            writer.Write(PauseAfter);
            writer.Write(DataLength);
            writer.Write(Data);
        }

        #endregion
    }
}