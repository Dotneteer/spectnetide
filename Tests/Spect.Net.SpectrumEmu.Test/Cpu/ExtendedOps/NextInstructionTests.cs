using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.ExtendedOps
{
    [TestClass]
    public class NextInstructionTests
    {
        /// <summary>
        /// SWAPNIB: 0xED 0x23
        /// </summary>
        [TestMethod]
        [DataRow(0x3D, 0xD3)]
        [DataRow(0xCA, 0xAC)]
        [DataRow(0xFB, 0xBF)]
        public void SWAPNIB_WorksAsExpected(int initial, int result)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x23           // SWAPNIB
            });
            var regs = m.Cpu.Registers;
            regs.A = (byte)initial;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)result);
            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// SWAPNIB: 0xED 0x23
        /// </summary>
        [TestMethod]
        public void SWAPNIB_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x23           // SWAPNIB
            });
            var regs = m.Cpu.Registers;

            // --- Act
            m.Run();

            // --- Assert
            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// MIRROR A: 0xED 0x24
        /// </summary>
        [TestMethod]
        [DataRow(0x55, 0xAA)]
        [DataRow(0x81, 0x81)]
        [DataRow(0xC4, 0x23)]
        public void MIRROR_A_WorksAsExpected(int initial, int result)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x24           // MIRROR A
            });
            var regs = m.Cpu.Registers;
            regs.A = (byte)initial;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)result);
            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// MIRROR A: 0xED 0x24
        /// </summary>
        [TestMethod]
        public void MIRROR_A_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x24           // MIRROR A
            });
            var regs = m.Cpu.Registers;

            // --- Act
            m.Run();

            // --- Assert
            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        public void LD_HL_SP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x25           // LD HL,SP
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x1111;
            regs.SP = 0x2222;

            // --- Act
            m.Run();

            // --- Assert

            regs.HL.ShouldBe((ushort)0x2222);
            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD HL,SP: 0xED 0x25
        /// </summary>
        [TestMethod]
        public void LD_HL_SP_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x25           // LD HL,SP
            });
            var regs = m.Cpu.Registers;

            // --- Act
            m.Run();

            // --- Assert
            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// MIRROR DE: 0xED 0x26
        /// </summary>
        [TestMethod]
        [DataRow(0x5555, 0xAAAA)]
        [DataRow(0x8100, 0x0081)]
        [DataRow(0xC401, 0x8023)]
        public void MIRROR_DE_WorksAsExpected(int initial, int result)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x26           // MIRROR DE
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)initial;

            // --- Act
            m.Run();

            // --- Assert

            regs.DE.ShouldBe((ushort)result);
            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// MIRROR DE: 0xED 0x26
        /// </summary>
        [TestMethod]
        public void MIRROR_DE_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x26           // MIRROR DE
            });
            var regs = m.Cpu.Registers;

            // --- Act
            m.Run();

            // --- Assert
            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// MIRROR DE: 0xED 0x26
        /// </summary>
        [TestMethod]
        [DataRow(0x4444, 0x5555, 0x16C1, 0x3E94)]
        [DataRow(0x5555, 0x4444, 0x16C1, 0x3E94)]
        [DataRow(0xEEEE, 0xDDDD, 0xCF11, 0xB976)]
        public void MUL_WorksAsExpected(int hl, int de, int resHl, int resDe)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x30           // MUL
            });
            var regs = m.Cpu.Registers;
            regs.HL = (ushort)hl;
            regs.DE = (ushort)de;

            // --- Act
            m.Run();

            // --- Assert

            regs.HL.ShouldBe((ushort)resHl);
            regs.DE.ShouldBe((ushort)resDe);
            m.ShouldKeepRegisters(except: "DE, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }


        /// <summary>
        /// MUL: 0xED 0x30
        /// </summary>
        [TestMethod]
        public void MUL_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x30           // MUL
            });
            var regs = m.Cpu.Registers;

            // --- Act
            m.Run();

            // --- Assert
            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// ADD HL,A: 0xED 0x31
        /// </summary>
        [TestMethod]
        [DataRow(0x4444, 0x55, 0x4499)]
        [DataRow(0x5555, 0x44, 0x5599)]
        [DataRow(0xE3E4, 0xD5, 0xE4B9)]
        public void ADD_HL_A_WorksAsExpected(int hl, int a, int result)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x31           // ADD HL,A
            });
            var regs = m.Cpu.Registers;
            regs.HL = (ushort)hl;
            regs.A = (byte)a;

            // --- Act
            m.Run();

            // --- Assert

            regs.HL.ShouldBe((ushort)result);
            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// ADD HL,1: 0xED 0x31
        /// </summary>
        [TestMethod]
        public void ADD_HL_A_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x31           // ADD HL,A
            });
            var regs = m.Cpu.Registers;

            // --- Act
            m.Run();

            // --- Assert
            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }
    }
}
