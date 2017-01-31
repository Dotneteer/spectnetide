using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core
{
    [TestClass]
    public class StandardOpTests0xC0
    {
        /// <summary>
        /// RET NZ: 0xC0
        /// </summary>
        [TestMethod]
        public void RET_NZ_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xB7,             // OR A
                0xC0,             // RET NZ
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
        /// POP BC: 0xC1
        /// </summary>
        [TestMethod]
        public void POP_BC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x52, 0x23, // LD HL,2352H
                0xE5,             // PUSH HL
                0xC1              // POP BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.BC.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "HL, BC");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Ticks.ShouldBe(31ul);
        }

        /// <summary>
        /// JP NZ,NN: 0xC2
        /// </summary>
        [TestMethod]
        public void JP_NZ_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xC2, 0x07, 0x00, // JP NZ,0007H
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
        /// JP NN: 0xC3
        /// </summary>
        [TestMethod]
        public void JP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xC3, 0x06, 0x00, // JP 0006H
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

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Ticks.ShouldBe(28ul);
        }

        /// <summary>
        /// CALL NZ: 0xC4
        /// </summary>
        [TestMethod]
        public void CALL_NZ_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xC4, 0x07, 0x00, // CALL NZ,0007H
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
        /// PUSH BC: 0xC5
        /// </summary>
        [TestMethod]
        public void PUSH_BC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x52, 0x23, // LD BC,2352H
                0xC5,             // PUSH BC
                0xE1              // POP HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.HL.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "HL, BC");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Ticks.ShouldBe(31ul);
        }

        /// <summary>
        /// RET Z: 0xC8
        /// </summary>
        [TestMethod]
        public void RET_Z_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xAF,             // XOR A
                0xC8,             // RET Z
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x00);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(43ul);
        }

        /// <summary>
        /// JP Z,NN: 0xCA
        /// </summary>
        [TestMethod]
        public void JP_Z_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xAF,             // XOR A
                0xCA, 0x07, 0x00, // JP Z,0007H
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
        /// CALL Z: 0xCC
        /// </summary>
        [TestMethod]
        public void CALL_Z_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xAF,             // XOR A
                0xCC, 0x07, 0x00, // CALL Z,0007H
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
        /// CALL NN: 0xCD
        /// </summary>
        [TestMethod]
        public void CALL_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x3E, 0xA3,       // LD A,A3H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xA3);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(45ul);
        }


    }
}
