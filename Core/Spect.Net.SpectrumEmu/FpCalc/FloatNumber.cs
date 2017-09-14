using System;
using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.FpCalc
{
    /// <summary>
    /// This class contains helpers that manage ZX Spectrum float numbers
    /// </summary>
    public class FloatNumber
    {
        public static float FromBytes(IList<byte> bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (bytes.Count != 5)
            {
                throw new ArgumentException("A float number must be exactly 5 bytes", nameof(bytes));
            }

            if (bytes[0] == 0)
            {
                // --- Simple integer form
                var neg = bytes[1] == 0xFF;
                return (bytes[2] + bytes[3] * 0x100) * (neg ? -1 : 1);
            }

            var sign = (bytes[1] & 0x80) == 0 ? 1 : -1;
            var mant = (uint)(((bytes[1] & 0x7F | 0x80) << 24) | (bytes[2] << 16) | (bytes[3] << 8) | bytes[4]);
            var exp = bytes[0] - 128 - 32;
            return (float)(sign * mant * Math.Pow(2.0, exp));
        }

        public static float FromCompactBytes(IList<byte> bytes)
        {
            var copyFrom = 1;
            var exp = bytes[0] & 0x3F;
            if (exp == 0)
            {
                exp = bytes[1];
                copyFrom = 2;
            }
            exp += 0x50;
            var newBytes = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00};
            newBytes[0] = (byte)exp;
            var idx = 1;
            for (var i = copyFrom; i < bytes.Count; i++)
            {
                newBytes[idx++] = bytes[i];
            }
            return FromBytes(newBytes);
        }
    }
}