using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.IndexedOps
{
    [TestClass]
    public class IxIndexedOpsTests
    {
        /// <summary>
        /// ADD IX,BC: 0xDD 0x09
        /// </summary>
        [TestMethod]
        public void ADD_IX_BC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0x01, 0x34, 0x12,       // LD BC,1234H
                0xDD, 0x09              // ADD IX,BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2335);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, BC, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Ticks.ShouldBe(39ul);
        }

        /// <summary>
        /// ADD IX,DE: 0xDD 0x19
        /// </summary>
        [TestMethod]
        public void ADD_IX_DE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0x11, 0x34, 0x12,       // LD DE,1234H
                0xDD, 0x19              // ADD IX,DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2335);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, DE, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Ticks.ShouldBe(39ul);
        }

        /// <summary>
        /// LD IX,NN: 0xDD 0x21
        /// </summary>
        [TestMethod]
        public void LD_IX_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11 // LD IX,1101H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1101);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(14ul);
        }

        /// <summary>
        /// ADD IX,IX: 0xDD 0x29
        /// </summary>
        [TestMethod]
        public void ADD_IX_IX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0xDD, 0x29              // ADD IX,IX
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2202);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(29ul);
        }

        /// <summary>
        /// ADD IX,SP: 0xDD 0x39
        /// </summary>
        [TestMethod]
        public void ADD_IX_SP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0x31, 0x34, 0x12,       // LD SP,1234H
                0xDD, 0x39              // ADD IX,SP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2335);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, SP, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Ticks.ShouldBe(39ul);
        }
    }
}
