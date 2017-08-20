using System;

namespace Spect.Net.SpectrumEmu.FpCalc
{
    /// <summary>
    /// This class contains helpers that manage ZX Spectrum float numbers
    /// </summary>
    public class FloatNumber
    {
        public static float FromBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (bytes.Length != 5)
            {
                throw new ArgumentException("A float number must be exactly 5 bytes", nameof(bytes));
            }

            if (bytes[0] == 0)
            {
                // --- Simple integer form
                var neg = bytes[1] == 0xFF;
                return (bytes[2] + bytes[3] * 0x100) * (neg ? -1 : 1);
            }
            return 0F;
        }
    }
}