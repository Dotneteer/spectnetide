using System.IO;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This will enable the emulators to display a message for a given time.
    /// </summary>
    /// <remarks>
    /// This should not stop the tape and it should not make silence. If the 
    /// time is 0 then the emulator should wait for the user to press a key.
    /// </remarks>
    public class TzxMessageDataBlock: TzxDataBlockBase
    {
        /// <summary>
        /// Time (in seconds) for which the message should be displayed
        /// </summary>
        public byte Time { get; set; }

        /// <summary>
        /// Length of the description
        /// </summary>
        public byte MessageLength { get; set; }

        /// <summary>
        /// The description bytes
        /// </summary>
        public byte[] Message;

        /// <summary>
        /// The string form of description
        /// </summary>
        public string MessageText => ToAsciiString(Message);

        /// <summary>
        /// The ID of the block
        /// </summary>
        public override int BlockId => 0x31;

        /// <summary>
        /// Reads the content of the block from the specified binary stream.
        /// </summary>
        /// <param name="reader">Stream to read the block from</param>
        public override void ReadFrom(BinaryReader reader)
        {
            Time = reader.ReadByte();
            MessageLength = reader.ReadByte();
            Message = reader.ReadBytes(MessageLength);
        }

        /// <summary>
        /// Writes the content of the block to the specified binary stream.
        /// </summary>
        /// <param name="writer">Stream to write the block to</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Time);
            writer.Write(MessageLength);
            writer.Write(Message);
        }
    }
}