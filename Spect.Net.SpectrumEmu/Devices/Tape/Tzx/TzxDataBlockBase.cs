using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Spect.Net.SpectrumEmu.Devices.Tape.Tzx
{
    /// <summary>
    /// This class describes a TZX Block
    /// </summary>
    public abstract class TzxDataBlockBase : ITzxSerialization
    {
        /// <summary>
        /// The ID of the block
        /// </summary>
        public abstract int BlockId { get; }

        /// <summary>
        /// The total number of Z80 CPU tacts while this block is played
        /// </summary>
        public int TotalTacts { get; protected set; }

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

        /// <summary>
        /// Tests if the data block support tape playback
        /// </summary>
        public bool SupportPlayback => typeof(ISupportsTapePlayback).GetTypeInfo()
            .IsAssignableFrom(GetType().GetTypeInfo());

        /// <summary>
        /// Reads the specified number of words from the reader.
        /// </summary>
        /// <param name="reader">Reader to obtain the input from</param>
        /// <param name="count">Number of words to get</param>
        /// <returns>Word array read from the input</returns>
        public static ushort[] ReadWords(BinaryReader reader, int count)
        {
            var result = new ushort[count];
            var bytes = reader.ReadBytes(2 * count);
            for (var i = 0; i < count; i++)
            {
                result[i] = (ushort)(bytes[i * 2] + bytes[i * 2 + 1] << 8);
            }
            return result;
        }

        /// <summary>
        /// Writes the specified array of words to the writer
        /// </summary>
        /// <param name="writer">Output</param>
        /// <param name="words">Word array</param>
        public static void WriteWords(BinaryWriter writer, ushort[] words)
        {
            foreach (var word in words)
            {
                writer.Write(word);
            }
        }

        /// <summary>
        /// Converts the provided bytes to an ASCII string
        /// </summary>
        /// <param name="bytes">Bytes to convert</param>
        /// <returns>ASCII string representation</returns>
        public static string ToAsciiString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(Convert.ToChar(b));
            }
            return sb.ToString();
        }
    }
}