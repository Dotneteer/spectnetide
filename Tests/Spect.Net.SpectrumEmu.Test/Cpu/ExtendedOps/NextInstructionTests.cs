using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
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
        /// TEST N: 0xED 0x27
        /// </summary>
        [TestMethod]
        [DataRow(0x00, 0x00, 0x54)]
        [DataRow(0x81, 0x83, 0x94)]
        [DataRow(0x26, 0x34, 0x34)]
        public void TEST_N_WorksAsExpected(int a, int n, int f)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x27, (byte)n   // TEST N
            });
            var regs = m.Cpu.Registers;
            regs.A = (byte)a;

            // --- Act
            m.Run();

            // --- Assert

            regs.F.ShouldBe((byte)f);
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// TEST N: 0xED 0x27
        /// </summary>
        [TestMethod]
        public void TEST_N_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x27           // TEST N
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
        /// ADD HL,A: 0xED 0x31
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

        /// <summary>
        /// ADD DE,A: 0xED 0x32
        /// </summary>
        [TestMethod]
        [DataRow(0x4444, 0x55, 0x4499)]
        [DataRow(0x5555, 0x44, 0x5599)]
        [DataRow(0xE3E4, 0xD5, 0xE4B9)]
        public void ADD_DE_A_WorksAsExpected(int de, int a, int result)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x32           // ADD DE,A
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)de;
            regs.A = (byte)a;

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
        /// ADD DE,A: 0xED 0x32
        /// </summary>
        [TestMethod]
        public void ADD_DE_A_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x32           // ADD DE,A
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
        /// ADD BC,A: 0xED 0x33
        /// </summary>
        [TestMethod]
        [DataRow(0x4444, 0x55, 0x4499)]
        [DataRow(0x5555, 0x44, 0x5599)]
        [DataRow(0xE3E4, 0xD5, 0xE4B9)]
        public void ADD_BC_A_WorksAsExpected(int bc, int a, int result)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x33           // ADD BC,A
            });
            var regs = m.Cpu.Registers;
            regs.BC = (ushort)bc;
            regs.A = (byte)a;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)result);
            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// ADD BC,A: 0xED 0x33
        /// </summary>
        [TestMethod]
        public void ADD_BC_A_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x33           // ADD BC,A
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
        /// INC DEHL: 0xED 0x37
        /// </summary>
        [TestMethod]
        [DataRow(0x1000, 0xFFFF, 0x1001, 0x0000)]
        [DataRow(0x5555, 0x4444, 0x5555, 0x4445)]
        [DataRow(0xFFFF, 0xFFFF, 0x0000, 0x0000)]
        public void INC_DEHL_WorksAsExpected(int de, int hl, int resDe, int resHl)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x37           // INC DEHL
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)de;
            regs.HL = (ushort)hl;

            // --- Act
            m.Run();

            // --- Assert

            regs.DE.ShouldBe((ushort)resDe);
            regs.HL.ShouldBe((ushort) resHl);
            m.ShouldKeepRegisters(except: "DE,HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// INC DEHL: 0xED 0x37
        /// </summary>
        [TestMethod]
        public void INC_DEHL_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x37           // INC DEHL
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
        /// DEC DEHL: 0xED 0x38
        /// </summary>
        [TestMethod]
        [DataRow(0x1001, 0x0000, 0x1000, 0xFFFF)]
        [DataRow(0x5555, 0x4444, 0x5555, 0x4443)]
        [DataRow(0x0000, 0x0000, 0xFFFF, 0xFFFF)]
        public void DEC_DEHL_WorksAsExpected(int de, int hl, int resDe, int resHl)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x38           // DEC DEHL
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)de;
            regs.HL = (ushort)hl;

            // --- Act
            m.Run();

            // --- Assert

            regs.DE.ShouldBe((ushort)resDe);
            regs.HL.ShouldBe((ushort)resHl);
            m.ShouldKeepRegisters(except: "DE,HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// DEC DEHL: 0xED 0x38
        /// </summary>
        [TestMethod]
        public void DEC_DEHL_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x38           // DEC DEHL
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
        /// ADD DEHL,A: 0xED 0x39
        /// </summary>
        [TestMethod]
        [DataRow(0x1001, 0x0000, 0xE8, 0x1001, 0x00E8)]
        [DataRow(0x5555, 0x4444, 0x00, 0x5555, 0x4444)]
        [DataRow(0xABCD, 0xFFF0, 0x37, 0xABCE, 0x0027)]
        public void ADD_DEHL_A_WorksAsExpected(int de, int hl, int a, int resDe, int resHl)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x39           // ADD DEHL,A
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)de;
            regs.HL = (ushort)hl;
            regs.A = (byte) a;

            // --- Act
            m.Run();

            // --- Assert

            regs.DE.ShouldBe((ushort)resDe);
            regs.HL.ShouldBe((ushort)resHl);
            m.ShouldKeepRegisters(except: "DE,HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// ADD DEHL,A: 0xED 0x39
        /// </summary>
        [TestMethod]
        public void ADD_DEHL_A_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x39           // ADD DEHL,A
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
        /// ADD DEHL,BC: 0xED 0x3A
        /// </summary>
        [TestMethod]
        [DataRow(0x1001, 0x0000, 0xE827, 0x1001, 0xE827)]
        [DataRow(0x5555, 0x4444, 0x0034, 0x5555, 0x4478)]
        [DataRow(0xABCD, 0xFFF0, 0x37FA, 0xABCE, 0x37EA)]
        public void ADD_DEHL_BC_WorksAsExpected(int de, int hl, int bc, int resDe, int resHl)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x3A           // ADD DEHL,BC
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)de;
            regs.HL = (ushort)hl;
            regs.BC = (ushort)bc;

            // --- Act
            m.Run();

            // --- Assert

            regs.DE.ShouldBe((ushort)resDe);
            regs.HL.ShouldBe((ushort)resHl);
            m.ShouldKeepRegisters(except: "DE,HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// ADD DEHL,BC: 0xED 0x3A
        /// </summary>
        [TestMethod]
        public void ADD_DEHL_BC_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x3A           // ADD DEHL,BC
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
        /// ADD DEHL,NN: 0xED 0x3B
        /// </summary>
        [TestMethod]
        [DataRow(0x1001, 0x0000, 0xE827, 0x1001, 0xE827)]
        [DataRow(0x5555, 0x4444, 0x0034, 0x5555, 0x4478)]
        [DataRow(0xABCD, 0xFFF0, 0x37FA, 0xABCE, 0x37EA)]
        public void ADD_DEHL_NN_WorksAsExpected(int de, int hl, int nn, int resDe, int resHl)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x3B, (byte)nn, (byte)(nn >> 8)  // ADD DEHL,NN
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)de;
            regs.HL = (ushort)hl;

            // --- Act
            m.Run();

            // --- Assert

            regs.DE.ShouldBe((ushort)resDe);
            regs.HL.ShouldBe((ushort)resHl);
            m.ShouldKeepRegisters(except: "DE,HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(14L);
        }

        /// <summary>
        /// ADD DEHL,NN: 0xED 0x3B
        /// </summary>
        [TestMethod]
        public void ADD_DEHL_NN_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x3B           // ADD DEHL,NN
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
        /// SUB DEHL,A: 0xED 0x3C
        /// </summary>
        [TestMethod]
        [DataRow(0x1001, 0x0000, 0xE8, 0x1000, 0xFF18)]
        [DataRow(0x5555, 0x4444, 0x00, 0x5555, 0x4444)]
        [DataRow(0xABCD, 0xFFF0, 0x37, 0xABCD, 0xFFB9)]
        public void SUB_DEHL_A_WorksAsExpected(int de, int hl, int a, int resDe, int resHl)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x3C           // SUB DEHL,A
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)de;
            regs.HL = (ushort)hl;
            regs.A = (byte)a;

            // --- Act
            m.Run();

            // --- Assert

            regs.DE.ShouldBe((ushort)resDe);
            regs.HL.ShouldBe((ushort)resHl);
            m.ShouldKeepRegisters(except: "DE,HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// SUB DEHL,A: 0xED 0x3C
        /// </summary>
        [TestMethod]
        public void SUB_DEHL_A_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x3C           // SUB DEHL,A
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
        /// SUB DEHL,BC: 0xED 0x3D
        /// </summary>
        [TestMethod]
        [DataRow(0x1001, 0x0000, 0xE827, 0x1000, 0x17D9)]
        [DataRow(0x5555, 0x4444, 0x0034, 0x5555, 0x4410)]
        [DataRow(0xABCD, 0xFFF0, 0x37FA, 0xABCD, 0xC7F6)]
        public void SUB_DEHL_BC_WorksAsExpected(int de, int hl, int bc, int resDe, int resHl)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x3D           // SUB DEHL,BC
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)de;
            regs.HL = (ushort)hl;
            regs.BC = (ushort)bc;

            // --- Act
            m.Run();

            // --- Assert

            regs.DE.ShouldBe((ushort)resDe);
            regs.HL.ShouldBe((ushort)resHl);
            m.ShouldKeepRegisters(except: "DE,HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// SUB DEHL,BC: 0xED 0x3D
        /// </summary>
        [TestMethod]
        public void SUB_DEHL_BC_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x3D           // SUB DEHL,BC
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
        /// PUSH NN: 0xED 0x8A
        /// </summary>
        [TestMethod]
        public void PUSH_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd, true);
            m.InitCode(new byte[]
            {
                0xED, 0x8A, 0x52, 0x23, // PUSH 2352H
                0xE1                    // POP HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.HL.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(30L);
        }

        /// <summary>
        /// PUSH NN: 0xED 0x8A
        /// </summary>
        [TestMethod]
        public void PUSH_NN_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x8A           // PUSH NN
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
        /// POPX: 0xED 0x8B
        /// </summary>
        [TestMethod]
        public void POPX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd, true);
            m.InitCode(new byte[]
            {
                0xED, 0x8A, 0x52, 0x23, // PUSH 2352H
                0xED, 0x8B              // POPX
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SP.ShouldBe((ushort)0x0000);
            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except:"FFFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(28L);
        }

        /// <summary>
        /// POPX: 0xED 0x8B
        /// </summary>
        [TestMethod]
        public void POPX_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x8B           // POPX
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
        /// NEXTREG reg,val: 0xED 0x91
        /// </summary>
        [TestMethod]
        public void NEXTREG_WorksAsExpected()
        {
            // --- Arrange
            var tbblue = new FakeTbBlueDevice();
            var m = new Z80TestMachine(RunMode.UntilEnd, true, tbblue);
            m.InitCode(new byte[]
            {
                0xED, 0x91, 0x07, 0x23, // NEXTREG 0x07,0x23
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            tbblue.Register.ShouldBe((byte)0x07);
            tbblue.Value.ShouldBe((byte)0x23);
            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(14L);
        }

        /// <summary>
        /// NEXTREG reg,val: 0xED 0x91
        /// </summary>
        [TestMethod]
        public void NEXTREG_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x91           // NEXTREG reg,val
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
        /// NEXTREG reg,A: 0xED 0x92
        /// </summary>
        [TestMethod]
        public void NEXTREG_A_WorksAsExpected()
        {
            // --- Arrange
            var tbblue = new FakeTbBlueDevice();
            var m = new Z80TestMachine(RunMode.UntilEnd, true, tbblue);
            m.InitCode(new byte[]
            {
                0xED, 0x92, 0x07 // NEXTREG 0x07,A
            });

            // --- Act
            var regs = m.Cpu.Registers;
            regs.A = 0x3D;
            m.Run();

            // --- Assert
            tbblue.Register.ShouldBe((byte)0x07);
            tbblue.Value.ShouldBe((byte)0x3D);
            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// NEXTREG reg,A: 0xED 0x92
        /// </summary>
        [TestMethod]
        public void NEXTREG_A_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x92           // NEXTREG reg,A
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

        private class FakeTbBlueDevice : ITbBlueControlDevice
        {
            public byte Register { get; private set; }
            public byte Value { get; private set; }

            public void Reset() {}

            public IDeviceState GetState() => null;

            public void RestoreState(IDeviceState state) {}

            public void SelectTbBlueRegister(byte register) => Register = register;

            public void SetTbBlueValue(byte value) => Value = value;
        }
    }
}
