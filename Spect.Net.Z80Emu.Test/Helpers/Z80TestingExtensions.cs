using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.Z80Emu.Core;
// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Emu.Test.Helpers
{
    /// <summary>
    /// Extension methods for testing the Z80 and related classes
    /// </summary>
    public static class Z80TestingExtensions
    {
        /// <summary>
        /// Clones the current set of registers
        /// </summary>
        /// <param name="regs"></param>
        /// <returns></returns>
        public static Registers Clone(this Registers regs)
        {
            return new Registers
            {
                _AF_ = regs._AF_,
                _BC_ = regs._BC_,
                _DE_ = regs._DE_,
                _HL_ = regs._HL_,
                AF = regs.AF,
                BC = regs.BC,
                DE = regs.DE,
                HL = regs.HL,
                SP = regs.SP,
                PC = regs.PC,
                IX = regs.IX,
                IY = regs.IY,
                IR = regs.IR,
                MW = regs.MW
            };
        }

        /// <summary>
        /// Checks if all registers keep their original values, except the ones
        /// listed in <paramref name="except"/>
        /// </summary>
        /// <param name="machine">Z80 test machine</param>
        /// <param name="except">
        /// Comma separated list of register pairs to be omitted from checks</param>
        /// <returns>True, if all registers keep their values.</returns>
        /// <remarks>
        /// PC, MW, and R are never checked, as they generally change during code
        /// execution. You should test them manually.
        /// </remarks>
        public static void ShouldKeepRegisters(this Z80TestMachine machine, string except = null)
        {
            var before = machine.RegistersBeforeRun;
            var after = machine.Cpu.Registers;
            var exclude = except?.Split(',') ?? new string[0];
            exclude = exclude.Select(reg => reg.ToUpper().Trim()).ToArray();
            var differs = new List<string>();

            if (before._AF_ != after._AF_ && !exclude.Contains("AF'"))
            {
                differs.Add("AF'");
            }
            if (before._BC_ != after._BC_ && !exclude.Contains("BC'"))
            {
                differs.Add("BC'");
            }
            if (before._DE_ != after._DE_ && !exclude.Contains("DE'"))
            {
                differs.Add("DE'");
            }
            if (before._HL_ != after._HL_ && !exclude.Contains("HL'"))
            {
                differs.Add("HL'");
            }
            if (before.AF != after.AF &&
                !(exclude.Contains("AF") || exclude.Contains("A") || exclude.Contains("F")))
            {
                differs.Add("AF");
            }
            if (before.BC != after.BC &&
                !(exclude.Contains("BC") || exclude.Contains("B") || exclude.Contains("C")))
            {
                differs.Add("BC");
            }
            if (before.DE != after.DE &&
                !(exclude.Contains("DE") || exclude.Contains("D") || exclude.Contains("E")))
            {
                differs.Add("DE");
            }
            if (before.HL != after.HL &&
                !(exclude.Contains("HL") || exclude.Contains("H") || exclude.Contains("L")))
            {
                differs.Add("HL");
            }
            if (before.SP != after.SP && !exclude.Contains("SP"))
            {
                differs.Add("SP");
            }
            if (before.IX != after.IX && !exclude.Contains("IX"))
            {
                differs.Add("IX");
            }
            if (before.IY != after.IY && !exclude.Contains("IY"))
            {
                differs.Add("IY");
            }
            if (before.A != after.A && !exclude.Contains("A") && !exclude.Contains("AF"))
            {
                differs.Add("A");
            }
            if (before.F != after.F && !exclude.Contains("F") && !exclude.Contains("AF"))
            {
                differs.Add("F");
            }
            if (before.B != after.B && !exclude.Contains("B") && !exclude.Contains("BC"))
            {
                differs.Add("B");
            }
            if (before.C != after.C && !exclude.Contains("C") && !exclude.Contains("BC"))
            {
                differs.Add("C");
            }
            if (before.D != after.D && !exclude.Contains("D") && !exclude.Contains("DE"))
            {
                differs.Add("D");
            }
            if (before.E != after.E && !exclude.Contains("E") && !exclude.Contains("DE"))
            {
                differs.Add("E");
            }
            if (before.H != after.H && !exclude.Contains("H") && !exclude.Contains("HL"))
            {
                differs.Add("H");
            }
            if (before.L != after.L && !exclude.Contains("L") && !exclude.Contains("HL"))
            {
                differs.Add("L");
            }
            if (differs.Count == 0) return;
            Assert.Fail("The following registers are expected to remain intact, " +
                        $"but their values have been changed: {string.Join(", ", differs)}");
        }

        /// <summary>
        /// Tests if S flag keeps its value while running a test.
        /// </summary>
        /// <param name="machine">Z80 test machine</param>
        public static void ShouldKeepSFlag(this Z80TestMachine machine)
        {
            var before = (machine.RegistersBeforeRun.F & (byte) FlagsSetMask.S) != 0;
            var after = (machine.Cpu.Registers.F & (byte) FlagsSetMask.S) != 0;
            if (after == before)
            {
                return;
            }
            Assert.Fail($"S flag expected to keep its value, but it changed from {before} to {after}");
        }

        /// <summary>
        /// Tests if Z flag keeps its value while running a test.
        /// </summary>
        /// <param name="machine">Z80 test machine</param>
        public static void ShouldKeepZFlag(this Z80TestMachine machine)
        {
            var before = (machine.RegistersBeforeRun.F & (byte)FlagsSetMask.Z) != 0;
            var after = (machine.Cpu.Registers.F & (byte)FlagsSetMask.Z) != 0;
            if (after == before)
            {
                return;
            }
            Assert.Fail($"Z flag expected to keep its value, but it changed from {before} to {after}");
        }

        /// <summary>
        /// Tests if N flag keeps its value while running a test.
        /// </summary>
        /// <param name="machine">Z80 test machine</param>
        public static void ShouldKeepNFlag(this Z80TestMachine machine)
        {
            var before = (machine.RegistersBeforeRun.F & (byte)FlagsSetMask.N) != 0;
            var after = (machine.Cpu.Registers.F & (byte)FlagsSetMask.N) != 0;
            if (after == before)
            {
                return;
            }
            Assert.Fail($"N flag expected to keep its value, but it changed from {before} to {after}");
        }

        /// <summary>
        /// Tests if PV flag keeps its value while running a test.
        /// </summary>
        /// <param name="machine">Z80 test machine</param>
        public static void ShouldKeepPVFlag(this Z80TestMachine machine)
        {
            var before = (machine.RegistersBeforeRun.F & (byte)FlagsSetMask.PV) != 0;
            var after = (machine.Cpu.Registers.F & (byte)FlagsSetMask.PV) != 0;
            if (after == before)
            {
                return;
            }
            Assert.Fail($"PV flag expected to keep its value, but it changed from {before} to {after}");
        }

        /// <summary>
        /// Tests if H flag keeps its value while running a test.
        /// </summary>
        /// <param name="machine">Z80 test machine</param>
        public static void ShouldKeepHFlag(this Z80TestMachine machine)
        {
            var before = (machine.RegistersBeforeRun.F & (byte)FlagsSetMask.H) != 0;
            var after = (machine.Cpu.Registers.F & (byte)FlagsSetMask.H) != 0;
            if (after == before)
            {
                return;
            }
            Assert.Fail($"PV flag expected to keep its value, but it changed from {before} to {after}");
        }

        /// <summary>
        /// Tests if C flag keeps its value while running a test.
        /// </summary>
        /// <param name="machine">Z80 test machine</param>
        public static void ShouldKeepCFlag(this Z80TestMachine machine)
        {
            var before = (machine.RegistersBeforeRun.F & (byte)FlagsSetMask.C) != 0;
            var after = (machine.Cpu.Registers.F & (byte)FlagsSetMask.C) != 0;
            if (after == before)
            {
                return;
            }
            Assert.Fail($"C flag expected to keep its value, but it changed from {before} to {after}");
        }

        /// <summary>
        /// Check if the machine's memory keeps its previous values, except
        /// the addresses and address ranges specified in <paramref name="except"/>
        /// </summary>
        /// <param name="machine">Z80 test machine</param>
        /// <param name="except">
        /// Address ranges separated by comma
        /// </param>
        public static void ShouldKeepMemory(this Z80TestMachine machine, string except = null)
        {
            const int MAX_DEVS = 10;

            var ranges = new List<Tuple<ushort, ushort>>();
            var deviations = new List<ushort>();

            // --- Parse ranges
            var strRanges = except?.Split(',') ?? new string[0];
            foreach (var range in strRanges)
            {
                var blocks = range.Split('-');
                ushort lower = 0xffff;
                ushort upper = 0xffff;
                if (blocks.Length >= 1)
                {
                    ushort.TryParse(blocks[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lower);
                    upper = lower;
                }
                if (blocks.Length >= 2)
                {
                    ushort.TryParse(blocks[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out upper);
                }
                ranges.Add(new Tuple<ushort, ushort>(lower, upper));
            }

            // --- Check each byte of memory, ignoring the stack
            var upperMemoryBound = (int)machine.Cpu.Registers.SP;
            if (upperMemoryBound == 0) upperMemoryBound = ushort.MaxValue + 1;
            for (var idx = 0; idx < upperMemoryBound; idx++)
            {
                if (machine.Memory[(ushort) idx] == machine.MemoryBeforeRun[(ushort) idx])
                {
                    continue;
                }

                // --- Test allowed deviations
                var found = ranges.Any(range => idx >= range.Item1 && idx <= range.Item2);
                if (found) continue;

                // --- Report deviation
                deviations.Add((ushort)idx);
                if (deviations.Count >= MAX_DEVS) break;
            }

            if (deviations.Count > 0)
            {
                Assert.Fail("The following memory locations are expected to remain intact, " +
                    "but their values have been changed: " +
                    string.Join(", ", deviations.Select(d => d.ToString("X"))));
            }
        }
    }
}