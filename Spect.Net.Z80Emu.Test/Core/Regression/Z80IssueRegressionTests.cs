using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.Regression
{
    [TestClass]
    public class Z80IssueRegressionTests
    {
        /// <summary>
        /// ISSUE: When a Z80 indexed operation is executed (0xDD or 0xFD prefix),
        /// the IndexMode property of Z80 is not reset.
        /// </summary>
        [TestMethod]
        public void IndexModeIsResetAfterAnIndexedOperation()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0x21, 0x34, 0x12,       // LD HL,1234H

                // === The issue makes the CPU behave as if these operations were
                // === executed:
                // === LD IX,1101H
                // === LD IX,1234H (and not LD HL,1234H)
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1101);
            regs.HL.ShouldBe((ushort)0x1234);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, HL");
            m.ShouldKeepMemory();
        }

        [TestMethod]
        public void RrcaDoesNotWorkProperlyWith18()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x0F        // RRCA
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x09);

            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();
        }
    }
}
