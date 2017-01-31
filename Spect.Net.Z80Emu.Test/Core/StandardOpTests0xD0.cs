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
        /// OUT (N), A: 0xD3
        /// </summary>
        [TestMethod]
        public void OUT_N_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xD3, 0x28        // OUT (N),A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1628);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x16);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(18ul);
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
        /// SUB N: 0xD6
        /// </summary>
        [TestMethod]
        public void SUB_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0xD6, 0x24  // SUB 24H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x12);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(14ul);
        }

        /// <summary>
        /// RST 10H: 0xD7
        /// </summary>
        [TestMethod]
        public void RST_10_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xD7        // RST 10H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x12);

            regs.SP.ShouldBe((ushort)0xFFFE);
            regs.PC.ShouldBe((ushort)0x10);
            m.Memory[0xFFFE].ShouldBe((byte)0x03);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Ticks.ShouldBe(18ul);
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
        /// EXX: 0xD9
        /// </summary>
        [TestMethod]
        public void EXX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xD9 // EXX
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0xABCD;
            regs._BC_ = 0x2345;
            regs.DE = 0xBCDE;
            regs._DE_ = 0x3456;
            regs.HL = 0xCDEF;
            regs._HL_ = 0x4567;

            // --- Act
            m.Run();

            // --- Assert
            regs.BC.ShouldBe((ushort)0x2345);
            regs._BC_.ShouldBe((ushort)0xABCD);
            regs.DE.ShouldBe((ushort)0x3456);
            regs._DE_.ShouldBe((ushort)0xBCDE);
            regs.HL.ShouldBe((ushort)0x4567);
            regs._HL_.ShouldBe((ushort)0xCDEF);

            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0001);
            m.Cpu.Ticks.ShouldBe(4ul);
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
        /// IN A,(N): 0xDB
        /// </summary>
        [TestMethod]
        public void IN_A_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xDB, 0x34        // IN A,(34H)
            });
            m.IoInputSequence.Add(0xD5);

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xD5);
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1634);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xD5);
            m.IoAccessLog[0].IsOutput.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(18ul);
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

        /// <summary>
        /// SBC N: 0xDE
        /// </summary>
        [TestMethod]
        public void SBC_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x37,       // SCF
                0xDE, 0x24  // SBC 24H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x11);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Ticks.ShouldBe(18ul);
        }

        /// <summary>
        /// RST 18H: 0xDF
        /// </summary>
        [TestMethod]
        public void RST_18_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDF        // RST 18H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x12);

            regs.SP.ShouldBe((ushort)0xFFFE);
            regs.PC.ShouldBe((ushort)0x18);
            m.Memory[0xFFFE].ShouldBe((byte)0x03);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Ticks.ShouldBe(18ul);
        }
    }
}