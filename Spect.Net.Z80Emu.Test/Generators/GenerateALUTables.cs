using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Z80Emu.Core;

// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Emu.Test.Generators
{
    /// <summary>
    /// These unit test are used to generate C# code for ALU tables
    /// </summary>
    [TestClass]
    public class GenerateALUTables
    {
        [TestMethod]
        public void Generate8BitIncTable()
        {
            var table = new List<byte>();
            for (var b = 0; b < 0x100; b++)
            {
                var oldVal = (byte) b;
                var newVal = (byte)(oldVal + 1);

                var r3 = (newVal & FlagsSetMask.R3) != 0;
                var r5 = (newVal & FlagsSetMask.R5) != 0;
                var s = (newVal & 0x80) != 0;
                var z = newVal == 0;
                var h = (oldVal & 0x0F) == 0x0F;
                var pv = oldVal == 0x7F;

                // C is unaffected, we keep it false

                var flags =
                    (r3 ? FlagsSetMask.R3 : 0) |
                    (r5 ? FlagsSetMask.R5 : 0) |
                    (s ? FlagsSetMask.S : 0) |
                    (z ? FlagsSetMask.Z : 0) |
                    (h ? FlagsSetMask.H : 0) |
                    (pv ? FlagsSetMask.PV : 0);
                table.Add((byte)flags);
            }
            Display(table);
        }

        [TestMethod]
        public void Generate8BitDecTable()
        {
            var table = new List<byte>();
            for (var b = 0; b < 0x100; b++)
            {
                var oldVal = (byte)b;
                var newVal = (byte)(oldVal - 1);

                var r3 = (newVal & FlagsSetMask.R3) != 0;
                var r5 = (newVal & FlagsSetMask.R5) != 0;
                var s = (newVal & 0x80) != 0;
                var z = newVal == 0;
                var h = (oldVal & 0x0F) == 0x00;
                var pv = oldVal == 0x80;

                // C is unaffected, we keep it false

                var flags =
                    (r3 ? FlagsSetMask.R3 : 0) |
                    (r5 ? FlagsSetMask.R5 : 0) |
                    (s ? FlagsSetMask.S : 0) |
                    (z ? FlagsSetMask.Z : 0) |
                    (h ? FlagsSetMask.H : 0) |
                    (pv ? FlagsSetMask.PV : 0) |
                    FlagsSetMask.N;
                table.Add((byte)flags);
            }
            Display(table);
        }

        [TestMethod]
        public void GenerateDAATable()
        {
            var table = new ushort[0x800];
            for (var b = 0; b < 0x100; b++)
            {
                var hNibble = b >> 4;
                var lNibble = b & 0x0F;

                for (var H = 0; H <= 1; H++)
                {
                    for (var N = 0; N <= 1; N++)
                    {
                        for (var C = 0; C <= 1; C++)
                        {
                            // --- Calculate DIFF and the new value of C Flag
                            var diff = 0x00;
                            var cAfter = 0;
                            if (C == 0)
                            {
                                if (hNibble >= 0 && hNibble <= 9 && lNibble >= 0 && lNibble <= 9)
                                {
                                    diff = H == 0 ? 0x00 : 0x06;
                                }
                                else if (hNibble >= 0 && hNibble <= 8 && lNibble >= 0x0A && lNibble <= 0xF)
                                {
                                    diff = 0x06;
                                }
                                else if (hNibble >= 0x0A && hNibble <= 0x0F && lNibble >= 0 && lNibble <= 9 && H == 0)
                                {
                                    diff = 0x60;
                                }
                                else if (hNibble >= 9 && hNibble <= 0x0F && lNibble >= 0x0A && lNibble <= 0xF)
                                {
                                    diff = 0x66;
                                    cAfter = 1;
                                }
                                else if (hNibble >= 0x0A && hNibble <= 0x0F && lNibble >= 0 && lNibble <= 9)
                                {
                                    if (H == 1) diff = 0x66;
                                    cAfter = 1;
                                }
                            }
                            else
                            {
                                // C == 1
                                cAfter = 1;
                                if (lNibble >= 0 && lNibble <= 9)
                                {
                                    diff = H == 0 ? 0x60 : 0x66;
                                }
                                else if (lNibble >= 0x0A && lNibble <= 0x0F)
                                {
                                    diff = 0x66;
                                }
                            }

                            // --- Calculate new value of H Flag
                            var hAfter = 0;
                            if ((lNibble >= 0x0A && lNibble <= 0x0F && N == 0)
                                || (lNibble >= 0 && lNibble <= 5 && N == 1 && H == 1))
                            {
                                hAfter = 1;
                            }

                            // --- Calculate new value of register A
                            var A = (N == 0 ? b + diff : b - diff) & 0xFF;

                            // --- Calculate other flags
                            var r3 = (A & FlagsSetMask.R3) != 0;
                            var r5 = (A & FlagsSetMask.R5) != 0;
                            var s = (A & 0x80) != 0;
                            var z = A == 0;
                            var aPar = 0;
                            var val = A;
                            for (var i = 0; i < 8; i++)
                            {
                                aPar += val & 0x01;
                                val >>= 1;
                            }
                            var pv = aPar%2 == 0;

                            // --- Calculate result
                            var fAfter =
                                (r3 ? FlagsSetMask.R3 : 0) |
                                (r5 ? FlagsSetMask.R5 : 0) |
                                (s ? FlagsSetMask.S : 0) |
                                (z ? FlagsSetMask.Z : 0) |
                                (pv ? FlagsSetMask.PV : 0) |
                                (N == 1 ? FlagsSetMask.N : 0) |
                                (hAfter == 1 ? FlagsSetMask.H : 0) |
                                (cAfter == 1 ? FlagsSetMask.C : 0);

                            var result = (ushort) (A << 8 | (byte) fAfter);
                            var fBefore = (byte) (H*4 + N*2 + C);
                            var idx = (fBefore << 8) + b;
                            table[idx] = result;
                        }
                    }
                }
            }
            Display(table);
        }

        [TestMethod]
        public void Generate8BitALULogOpTable()
        {
            var table = new byte[0x100];
            for (var b = 0; b < 0x100; b++)
            {
                var flags = (byte)(b & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
                table[b] = (byte)(flags | GetParity((byte)b));
            }
            table[0] |= FlagsSetMask.Z;
            Display(table);
        }

        [TestMethod]
        public void Generate8BitRLCTable()
        {
            var table = new List<byte>();
            for (var b = 0; b < 0x100; b++)
            {
                var rlcVal = b;
                rlcVal <<= 1;
                var cf = (rlcVal & 0x100) != 0 ? FlagsSetMask.C : 0;
                if (cf != 0)
                {
                    rlcVal = (rlcVal | 0x01) & 0xFF;
                }
                var flags = (byte)(rlcVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
                flags |= (byte)(cf | GetParity((byte)rlcVal));
                if (rlcVal == 0) flags |= FlagsSetMask.Z;
                table.Add(flags);
            }
            Display(table);
        }

        [TestMethod]
        public void Generate8BitRRCTable()
        {
            var table = new List<byte>();
            for (var b = 0; b < 0x100; b++)
            {
                var rlcVal = b;
                var cf = (rlcVal & 0x01) != 0 ? FlagsSetMask.C : 0;
                rlcVal >>= 1;
                if (cf != 0)
                {
                    rlcVal = (rlcVal | 0x80);
                }
                var flags = (byte)(rlcVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
                flags |= (byte)(cf | GetParity((byte)rlcVal));
                if (rlcVal == 0) flags |= FlagsSetMask.Z;
                table.Add(flags);
            }
            Display(table);
        }

        [TestMethod]
        public void Generate8BitRLWithCarryTable()
        {
            var table = new List<byte>();
            for (var b = 0; b < 0x100; b++)
            {
                var rlVal = b;
                rlVal <<= 1;
                rlVal++;
                var cf = (rlVal & 0x100) != 0 ? FlagsSetMask.C : 0;
                var flags = (byte)(rlVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
                flags |= (byte)(cf | GetParity((byte)rlVal));
                if ((rlVal & 0x1FF) == 0) flags |= FlagsSetMask.Z;
                table.Add(flags);
            }
            Display(table);
        }

        [TestMethod]
        public void Generate8BitRLWithNoCarryTable()
        {
            var table = new List<byte>();
            for (var b = 0; b < 0x100; b++)
            {
                var rlVal = b;
                rlVal <<= 1;
                var cf = (rlVal & 0x100) != 0 ? FlagsSetMask.C : 0;
                var flags = (byte)(rlVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
                flags |= (byte)(cf | GetParity((byte)rlVal));
                if ((rlVal & 0xFF) == 0)
                {
                    flags |= FlagsSetMask.Z;
                }
                table.Add(flags);
            }
            Display(table);
        }

        [TestMethod]
        public void Generate8BitRRWithCarryTable()
        {
            var table = new List<byte>();
            for (var b = 0; b < 0x100; b++)
            {
                var rrVal = b;
                var cf = (rrVal & 0x01) != 0 ? FlagsSetMask.C : 0;
                rrVal >>= 1;
                rrVal += 0x80;
                var flags = (byte)(rrVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
                flags |= (byte)(cf | GetParity((byte)rrVal));
                if (rrVal == 0) flags |= FlagsSetMask.Z;
                table.Add(flags);
            }
            Display(table);
        }

        [TestMethod]
        public void Generate8BitRRWithNoCarryTable()
        {
            var table = new List<byte>();
            for (var b = 0; b < 0x100; b++)
            {
                var rrVal = b;
                var cf = (rrVal & 0x01) != 0 ? FlagsSetMask.C : 0;
                rrVal >>= 1;
                var flags = (byte)(rrVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
                flags |= (byte)(cf | GetParity((byte)rrVal));
                if (rrVal == 0) flags |= FlagsSetMask.Z;
                table.Add(flags);
            }
            Display(table);
        }

        [TestMethod]
        public void Generate8BitSRATable()
        {
            var table = new List<byte>();
            for (var b = 0; b < 0x100; b++)
            {
                var sraVal = b;
                var cf = (sraVal & 0x01) != 0 ? FlagsSetMask.C : 0;
                sraVal = (sraVal >> 1) + (sraVal & 0x80);
                var flags = (byte)(sraVal & (FlagsSetMask.S | FlagsSetMask.R5 | FlagsSetMask.R3));
                flags |= (byte)(cf | GetParity((byte)sraVal));
                if ((sraVal & 0xFF) == 0) flags |= FlagsSetMask.Z;
                table.Add(flags);
            }
            Display(table);
        }

        private static void Display(IList<byte> table, int itemPerRow = 16)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < table.Count; i++)
            {
                sb.AppendFormat("0x{0:X2}", table[i]);
                if (i < table.Count - 1)
                {
                    sb.AppendFormat(", ");
                }
                if ((i + 1)%itemPerRow == 0)
                {
                    sb.AppendLine();
                }
            }
            Console.WriteLine(sb);
        }

        private static void Display(IList<ushort> table, int itemPerRow = 8)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < table.Count; i++)
            {
                sb.AppendFormat("0x{0:X4}", table[i]);
                if (i < table.Count - 1)
                {
                    sb.AppendFormat(", ");
                }
                if ((i + 1) % itemPerRow == 0)
                {
                    sb.AppendLine();
                }
            }
            Console.WriteLine(sb);
        }

        private static byte GetParity(byte value)
        {
            var parity = FlagsSetMask.PV;
            for (var i = value; i != 0; i >>= 1)
            {
                if ((i & 0x01) != 0) parity ^= FlagsSetMask.PV;
            }
            return parity;
        }
    }
}
