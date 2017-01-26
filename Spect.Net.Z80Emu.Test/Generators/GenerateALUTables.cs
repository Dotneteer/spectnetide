using System;
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
        public void GenerateInc8BitTable()
        {
            for (var b = 0; b < 256; b++)
            {
                var oldVal = (byte) b;
                var newVal = (byte)(oldVal + 1);

                var r3 = (newVal & (byte)FlagsSetMask.R3) != 0;
                var r5 = (newVal & (byte)FlagsSetMask.R5) != 0;
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
                Console.Write(flags.ToString("X"));
                Console.Write(", ");
            }
        }
    }
}
