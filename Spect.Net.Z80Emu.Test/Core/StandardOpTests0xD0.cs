using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core
{
    [TestClass]
    public class StandardOpTests0xD0
    {
        /// <summary>
        /// RET NC: 0xD0
        /// </summary>
        [TestMethod]
        public void RET_NC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x3F,             // CCF
                0xD0,             // RET NC
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x16);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(43ul);
        }

        /// <summary>
        /// POP DE: 0xD1
        /// </summary>
        [TestMethod]
        public void POP_DE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x52, 0x23, // LD HL,2352H
                0xE5,             // PUSH HL
                0xD1              // POP DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.DE.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "HL, DE");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Ticks.ShouldBe(31ul);
        }

        /// <summary>
        /// JP NC,NN: 0xD2
        /// </summary>
        [TestMethod]
        public void JP_NC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0x3F,             // CCF
                0xD2, 0x07, 0x00, // JP NC,0007H
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
        /// CALL NC: 0xD4
        /// </summary>
        [TestMethod]
        public void CALL_NC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0x3F,             // CCF
                0xD4, 0x07, 0x00, // CALL NC,0007H
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
        /// PUSH DE: 0xD5
        /// </summary>
        [TestMethod]
        public void PUSH_DE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x11, 0x52, 0x23, // LD DE,2352H
                0xD5,             // PUSH DE
                0xE1              // POP HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.HL.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "HL, DE");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Ticks.ShouldBe(31ul);
        }

        /// <summary>
        /// RET C: 0xD8
        /// </summary>
        [TestMethod]
        public void RET_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x37,             // SCF
                0xD8,             // RET C
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x16);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(43ul);
        }

        /// <summary>
        /// JP C,NN: 0xDA
        /// </summary>
        [TestMethod]
        public void JP_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0x37,             // SCF
                0xDA, 0x07, 0x00, // JP C,0007H
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
        /// CALL C: 0xDC
        /// </summary>
        [TestMethod]
        public void CALL_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0x37,             // SCF
                0xDC, 0x07, 0x00, // CALL C,0007H
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