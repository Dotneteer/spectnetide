using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;
using Z80TestMachine = Spect.Net.SpectrumEmu.Test.Helpers.Z80TestMachine;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.StandardOps
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(43L);
        }

        /// <summary>
        /// RET P: 0xF0
        /// </summary>
        [TestMethod]
        public void RET_P_DoesNotReturnWhenM()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
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

            regs.A.ShouldBe((byte)0x24);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(54);
        }

        /// <summary>
        /// POP AF: 0xF1
        /// </summary>
        [TestMethod]
        public void POP_AF_WorksAsExpected()
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
            m.Cpu.Tacts.ShouldBe(31L);
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

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Tacts.ShouldBe(32L);
        }

        /// <summary>
        /// DI: 0xF3
        /// </summary>
        [TestMethod]
        public void DI_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xF3 // DI
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.Cpu.IFF1.ShouldBeFalse();
            m.Cpu.IFF2.ShouldBeFalse();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0001);
            m.Cpu.Tacts.ShouldBe(4L);
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

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(49L);
        }

        /// <summary>
        /// CALL P: 0xF4
        /// </summary>
        [TestMethod]
        public void CALL_P_DoesNotCallWhenM()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
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

            regs.A.ShouldBe((byte)0x80);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(25);
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
            m.Cpu.Tacts.ShouldBe(21L);
        }

        /// <summary>
        /// OR N: 0xF6
        /// </summary>
        [TestMethod]
        public void OR_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xF6, 0x23  // OR 23H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x33);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(14L);
        }

        /// <summary>
        /// RST 30H: 0xF7
        /// </summary>
        [TestMethod]
        public void RST_30_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xF7        // RST 30H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x12);

            regs.SP.ShouldBe((ushort)0xFFFE);
            regs.PC.ShouldBe((ushort)0x30);
            m.Memory[0xFFFE].ShouldBe((byte)0x03);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(18L);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(43L);
        }

        /// <summary>
        /// RET M: 0xF8
        /// </summary>
        [TestMethod]
        public void RET_M_DoesNotReturnWhenP()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
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

            regs.A.ShouldBe((byte)0x24);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(54);
        }

        /// <summary>
        /// LD SP,HL: 0xF9
        /// </summary>
        [TestMethod]
        public void LD_SP_HL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0xF9              // LD SP,HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SP.ShouldBe((ushort)0x1000);
            m.ShouldKeepRegisters(except: "HL, SP");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(16L);
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

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Tacts.ShouldBe(32L);
        }

        /// <summary>
        /// EI: 0xFB
        /// </summary>
        [TestMethod]
        public void EI_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFB // EI
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.Cpu.IFF1.ShouldBeTrue();
            m.Cpu.IFF2.ShouldBeTrue();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0001);
            m.Cpu.Tacts.ShouldBe(4L);
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

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(49L);
        }

        /// <summary>
        /// CALL M: 0xFC
        /// </summary>
        [TestMethod]
        public void CALL_M_DoesNotCallWhenP()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
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

            regs.A.ShouldBe((byte)0x64);
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(25);
        }

        /// <summary>
        /// CP N: 0xFE
        /// </summary>
        [TestMethod]
        public void CP_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFE, 0x24 // CP 24H
            });
            m.Cpu.Registers.A = 0x36;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(7L);
        }

        /// <summary>
        /// RST 38H: 0xFF
        /// </summary>
        [TestMethod]
        public void RST_38_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xFF        // RST 38H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x12);

            regs.SP.ShouldBe((ushort)0xFFFE);
            regs.PC.ShouldBe((ushort)0x38);
            m.Memory[0xFFFE].ShouldBe((byte)0x03);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(18L);
        }
    }
}