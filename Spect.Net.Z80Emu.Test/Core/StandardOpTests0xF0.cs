using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core
{
    [TestClass]
    public class StandardOpTests0xF0
    {
        /// <summary>
        /// RET P: 0xF0
        /// </summary>
        [TestMethod]
        public void RET_P_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xF0,             // RET P
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x64);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(43ul);
        }

        /// <summary>
        /// POP AF: 0xF1
        /// </summary>
        [TestMethod]
        public void POP_HL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x52, 0x23, // LD BC,2352H
                0xC5,             // PUSH BC
                0xF1              // POP AF
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.AF.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "AF, BC");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Ticks.ShouldBe(31ul);
        }

        /// <summary>
        /// JP P,NN: 0xF2
        /// </summary>
        [TestMethod]
        public void JP_P_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xF2, 0x07, 0x00, // JP P,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xAA);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x000A);
            m.Cpu.Ticks.ShouldBe(32ul);
        }

        /// <summary>
        /// CALL P: 0xF4
        /// </summary>
        [TestMethod]
        public void CALL_P_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xF4, 0x07, 0x00, // CALL P,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x24);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Ticks.ShouldBe(49ul);
        }

        /// <summary>
        /// PUSH AF: 0xF5
        /// </summary>
        [TestMethod]
        public void PUSH_AF_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xF5,             // PUSH AF
                0xC1              // POP BC
            });
            m.Cpu.Registers.AF = 0x3456;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.BC.ShouldBe((ushort)0x3456);
            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(21ul);
        }

        /// <summary>
        /// RET M: 0xF8
        /// </summary>
        [TestMethod]
        public void RET_M_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xF8,             // RET M
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x80);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(43ul);
        }

        /// <summary>
        /// JP M,NN: 0xFA
        /// </summary>
        [TestMethod]
        public void JP_M_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xFA, 0x07, 0x00, // JP M,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xAA);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x000A);
            m.Cpu.Ticks.ShouldBe(32ul);
        }

        /// <summary>
        /// CALL M: 0xFC
        /// </summary>
        [TestMethod]
        public void CALL_M_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xFC, 0x07, 0x00, // CALL M,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x24);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Ticks.ShouldBe(49ul);
        }


    }
}