using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Spect.Net.SpectrumEmu.Tape.Tzx
{
    /// <summary>
    /// Represents the header of the TZX file
    /// </summary>
    public class TzxHeader: TzxDataBlockBase
    {
        public static IReadOnlyList<byte> TzxSignature = 
            new ReadOnlyCollection<byte>(new byte[] { 0x5A, 0x58, 0x54, 0x61, 0x70, 0x65, 0x21 });
        public byte[] Signature { get; private set; }
        public byte Eot { get; private set; }
        public byte MajorVersion { get; private set; }
        public ulong MinorVersion { get; private set; }

        public TzxHeader(byte majorVersion = 1, ulong minorVersion = 20)
        {
            Signature = TzxSignature.ToArray();
            Eot = 0x1A;
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
        }

        #region Overrides of TzxDataBlockBase

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x00;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Signature = reader.ReadBytes(7);
            Eot = reader.ReadByte();
            MajorVersion = reader.ReadByte();
            MinorVersion = reader.ReadByte();
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Signature);
            writer.Write(Eot);
            writer.Write(MajorVersion);
            writer.Write(MinorVersion);
        }

        #endregion
    }
}