using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
using Z80TestMachine = Spect.Net.Z80Emu.Test.Helpers.Z80TestMachine;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.StandardOps
{
    [TestClass]
    public class StandardOpTests0xE0
    {
        /// <summary>
        /// RET PO: 0xE0
        /// </summary>
        [TestMethod]
        public void RET_PO_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xE0,             // RET PO
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x54);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(43ul);
        }

        /// <summary>
        /// POP HL: 0xE1
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
            m.Cpu.Tacts.ShouldBe(31ul);
        }

        /// <summary>
        /// JP PO,NN: 0xE2
        /// </summary>
        [TestMethod]
        public void JP_PO_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xE2, 0x07, 0x00, // JP PO,0007H
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
            m.Cpu.Tacts.ShouldBe(32ul);
        }

        /// <summary>
        /// EX (SP),HL: 0xE3
        /// </summary>
        [TestMethod]
        public void EX_SPi_HL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x31, 0x00, 0x10, // LD SP, 1000H
                0x21, 0x34, 0x12, // LD HL, 1234H
                0xE3              // EX (SP),HL
            });
            m.Memory[0x1000] = 0x78;
            m.Memory[0x1001] = 0x56;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.HL.ShouldBe((ushort)0x5678);
            m.Memory[0x1000].ShouldBe((byte)0x34);
            m.Memory[0x1001].ShouldBe((byte)0x12);

            m.ShouldKeepRegisters(except: "SP, HL");
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(39ul);
        }

        /// <summary>
        /// CALL PO: 0xE4
        /// </summary>
        [TestMethod]
        public void CALL_PO_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xE4, 0x07, 0x00, // CALL PO,0007H
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

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(49ul);
        }

        /// <summary>
        /// PUSH HL: 0xE5
        /// </summary>
        [TestMethod]
        public void PUSH_HL_WorksAsExpected()
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
            m.Cpu.Tacts.ShouldBe(31ul);
        }

        /// <summary>
        /// AND N: 0xE6
        /// </summary>
        [TestMethod]
        public void AND_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xE6, 0x23  // AND 23H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x02);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(14ul);
        }

        /// <summary>
        /// RST 20H: 0xE7
        /// </summary>
        [TestMethod]
        public void RST_20_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xE7        // RST 20H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x12);

            regs.SP.ShouldBe((ushort)0xFFFE);
            regs.PC.ShouldBe((ushort)0x20);
            m.Memory[0xFFFE].ShouldBe((byte)0x03);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(18ul);
        }

        /// <summary>
        /// RET PE: 0xE8
        /// </summary>
        [TestMethod]
        public void RET_PE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xE8,             // RET PE
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x10);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(43ul);
        }

        /// <summary>
        /// JP (HL): 0xE9
        /// </summary>
        [TestMethod]
        public void JP_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL, 1000H
                0xE9              // JP (HL)
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.PC.ShouldBe((ushort)0x1000);

            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            m.Cpu.Tacts.ShouldBe(14ul);
        }

        /// <summary>
        /// JP PE,NN: 0xEA
        /// </summary>
        [TestMethod]
        public void JP_PE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xEA, 0x07, 0x00, // JP PE,0007H
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
            m.Cpu.Tacts.ShouldBe(32ul);
        }

        /// <summary>
        /// EX DE,HL: 0xEB
        /// </summary>
        [TestMethod]
        public void EX_DE_HL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x34, 0x12, // LD HL,1234H
                0x11, 0x78, 0x56, // LD DE,5678H
                0xEB              // EX DE,HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.DE.ShouldBe((ushort)0x1234);
            regs.HL.ShouldBe((ushort)0x5678);
            m.ShouldKeepRegisters(except: "HL, DE");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(24ul);
        }

        /// <summary>
        /// CALL PE: 0xEC
        /// </summary>
        [TestMethod]
        public void CALL_PE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xEC, 0x07, 0x00, // CALL PE,0007H
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

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(49ul);
        }

        /// <summary>
        /// XOR N: 0xEE
        /// </summary>
        [TestMethod]
        public void XOR_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xEE, 0x23  // XOR 23H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x31);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(14ul);
        }

        /// <summary>
        /// RST 28H: 0xEF
        /// </summary>
        [TestMethod]
        public void RST_28_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xEF        // RST 28H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x12);

            regs.SP.ShouldBe((ushort)0xFFFE);
            regs.PC.ShouldBe((ushort)0x28);
            m.Memory[0xFFFE].ShouldBe((byte)0x03);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(18ul);
        }
    }
}