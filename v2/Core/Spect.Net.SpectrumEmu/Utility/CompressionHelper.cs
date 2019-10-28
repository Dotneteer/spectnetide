using System.IO;
using System.IO.Compression;

namespace Spect.Net.SpectrumEmu.Utility
{
    /// <summary>
    /// This class provides helper functions for byte array compression and decompression
    /// </summary>
    public static class CompressionHelper
    {
        /// <summary>
        /// Compresses the specified source array 
        /// </summary>
        /// <param name="source">Source byte array</param>
        /// <returns>Compressed byte array</returns>
        public static byte[] CompressBytes(byte[] source)
        {
            using (var inputStream = new MemoryStream(source))
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var compressionStream = new DeflateStream(outputStream, CompressionMode.Compress))
                    {
                        inputStream.CopyTo(compressionStream);
                    }
                    return outputStream.GetBuffer();
                }
            }
        }

        /// <summary>
        /// Decompresses the specified byte array
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="finalSize">Optional final size</param>
        /// <returns>Decompressed byte array</returns>
        public static byte[] DecompressBytes(byte[] source, int? finalSize = null)
        {
            using (var inputStream = new MemoryStream(source))
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var compressionStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                    {
                        compressionStream.CopyTo(outputStream);
                    }

                    if (finalSize == null) return outputStream.GetBuffer();
                    var bufferLength = outputStream.GetBuffer().Length;
                    if (finalSize > bufferLength)
                    {
                        finalSize = bufferLength;
                    }

                    var buffer = new byte[finalSize.Value];
                    var sourceBuffer = outputStream.GetBuffer();
                    for (var i = 0; i < finalSize; i++)
                    {
                        buffer[i] = sourceBuffer[i];
                    }

                    return buffer;
                }
            }
        }
    }
}