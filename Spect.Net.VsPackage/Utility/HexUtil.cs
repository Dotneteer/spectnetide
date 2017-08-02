namespace Spect.Net.VsPackage.Utility
{
    /// <summary>
    /// This class implement hexadecimal conversion utilities
    /// </summary>
    public static class HexUtil
    {
        private static readonly string[] s_HexBytes = new string[0x100];

        static HexUtil()
        {
            for (var i = 0x00; i < 0x100; i++)
            {
                s_HexBytes[i] = $"{i:X2}";
            }
        }

        /// <summary>
        /// Gets the two-digit hexadecimal form of the byte
        /// </summary>
        /// <param name="data">Byte to convert to hexadecimal string</param>
        /// <returns>Hexadecimal string representation of the input byte</returns>
        public static string Byte(byte data) => s_HexBytes[data];

        /// <summary>
        /// Gets the two-digit hexadecimal form of the byte
        /// </summary>
        /// <param name="data">Byte to convert to hexadecimal string</param>
        /// <returns>Hexadecimal string representation of the input byte</returns>
        public static string AsHexaByte(this byte data) => s_HexBytes[data];

        /// <summary>
        /// Gets the four-digit hexadecimal form of the specified word
        /// </summary>
        /// <param name="data">UShort to convert to hexadecimal</param>
        /// <returns>Hexadecimal string representation of the input UShort</returns>
        public static string Word(ushort data)
        {
            var low = (byte)(data & 0xFF);
            var high = (byte)(data >> 8);
            return high.AsHexaByte() + low.AsHexaByte();
        }

        /// <summary>
        /// Gets the four-digit hexadecimal form of the specified word
        /// </summary>
        /// <param name="data">UShort to convert to hexadecimal</param>
        /// <returns>Hexadecimal string representation of the input UShort</returns>
        public static string AsHexWord(this ushort data) => Word(data);

        /// <summary>
        /// Gets the four-digit hexadecimal form of the specified word
        /// </summary>
        /// <param name="data">UShort to convert to hexadecimal</param>
        /// <returns>Hexadecimal string representation of the input UShort</returns>
        public static string Word(int data) => Word((ushort) data);

        /// <summary>
        /// Gets the four-digit hexadecimal form of the specified word
        /// </summary>
        /// <param name="data">UShort to convert to hexadecimal</param>
        /// <returns>Hexadecimal string representation of the input UShort</returns>
        public static string AsHexWord(this int data) => Word((ushort)data);
    }
}