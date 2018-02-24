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
        /// ADD HL,NNNN: 0xED 0x34
        /// </summary>
        [TestMethod]
        [DataRow(0x5555, 0xAAAA, 0xFFFF)]
        [DataRow(0x8100, 0x0081, 0x8181)]
        [DataRow(0xC401, 0x8023, 0x4424)]
        public void ADD_HL_NN_WorksAsExpected(int hl, int nn, int result)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x34, (byte)nn, (byte)(nn >> 8)  // ADD HL,NN
            });
            var regs = m.Cpu.Registers;
            regs.HL = (ushort)hl;

            // --- Act
            m.Run();

            // --- Assert

            regs.HL.ShouldBe((ushort)result);
            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// ADD HL,NN: 0xED 0x34
        /// </summary>
        [TestMethod]
        public void ADD_HL_NN_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x34   // ADD HL,NN
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
        /// ADD DE,NNNN: 0xED 0x35
        /// </summary>
        [TestMethod]
        [DataRow(0x5555, 0xAAAA, 0xFFFF)]
        [DataRow(0x8100, 0x0081, 0x8181)]
        [DataRow(0xC401, 0x8023, 0x4424)]
        public void ADD_DE_NN_WorksAsExpected(int de, int nn, int result)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x35, (byte)nn, (byte)(nn >> 8)  // ADD DE,NN
            });
            var regs = m.Cpu.Registers;
            regs.DE = (ushort)de;

            // --- Act
            m.Run();

            // --- Assert

            regs.DE.ShouldBe((ushort)result);
            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// ADD DE,NN: 0xED 0x35
        /// </summary>
        [TestMethod]
        public void ADD_DE_NN_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x35   // ADD DE,NN
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
        /// ADD BC,NNNN: 0xED 0x36
        /// </summary>
        [TestMethod]
        [DataRow(0x5555, 0xAAAA, 0xFFFF)]
        [DataRow(0x8100, 0x0081, 0x8181)]
        [DataRow(0xC401, 0x8023, 0x4424)]
        public void ADD_BC_NN_WorksAsExpected(int bc, int nn, int result)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x36, (byte)nn, (byte)(nn >> 8)  // ADD BC,NN
            });
            var regs = m.Cpu.Registers;
            regs.BC = (ushort)bc;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)result);
            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// ADD BC,NN: 0xED 0x36
        /// </summary>
        [TestMethod]
        public void ADD_BC_NN_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x36   // ADD BC,NN
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
        /// OUTINB: 0xED 0x90
        /// </summary>
        [TestMethod]
        public void OUTINB_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x90 // OUTINB
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x10CC;
            regs.HL = 0x1000;
            m.Memory[regs.HL] = 0x29;

            // --- Act
            m.Run();

            // --- Assert

            regs.B.ShouldBe((byte)0x10);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x1001);

            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x10CC);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x29);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// OUTINB: 0xED 0x90
        /// </summary>
        [TestMethod]
        public void OUTINB_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x90           // OUTINB
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

        /// <summary>
        /// PIXELDN: 0xED 0x93
        /// </summary>
        [TestMethod]
        [DataRow(0x4000, 0x4100)]
        [DataRow(0x401F, 0x411F)]
        [DataRow(0x411E, 0x421E)]
        [DataRow(0x471F, 0x403F)]
        [DataRow(0x471E, 0x403E)]
        [DataRow(0x47E2, 0x4802)]
        [DataRow(0x47FF, 0x481F)]
        [DataRow(0x491E, 0x4A1E)]
        [DataRow(0x4F1F, 0x483F)]
        [DataRow(0x4F1E, 0x483E)]
        [DataRow(0x4FE2, 0x5002)]
        [DataRow(0x4FFF, 0x501F)]
        public void PIXELDN_WorksAsExpected(int orig, int down)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x93           // PIXELDN
            });
            var regs = m.Cpu.Registers;
            regs.HL = (ushort) orig;

            // --- Act
            m.Run();

            // --- Assert
            regs.HL.ShouldBe((ushort)down);
            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// PIXELDN: 0xED 0x93
        /// </summary>
        [TestMethod]
        public void PIXELDN_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x93           // PIXELDN
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
        /// PIXELAD: 0xED 0x94
        /// </summary>
        [TestMethod]
        [DataRow(0x00, 0x00, 0x4000)]
        [DataRow(0x00, 0xF7, 0x401E)]
        [DataRow(0x00, 0xFE, 0x401F)]
        [DataRow(0x06, 0x00, 0x4600)]
        [DataRow(0x06, 0xF7, 0x461E)]
        [DataRow(0x06, 0xFE, 0x461F)]
        [DataRow(0x0C, 0x00, 0x4420)]
        [DataRow(0x0C, 0xF7, 0x443E)]
        [DataRow(0x0C, 0xFE, 0x443F)]
        [DataRow(0x40, 0x00, 0x4800)]
        [DataRow(0x40, 0xF7, 0x481E)]
        [DataRow(0x40, 0xFE, 0x481F)]
        [DataRow(0x46, 0x00, 0x4E00)]
        [DataRow(0x46, 0xF7, 0x4E1E)]
        [DataRow(0x46, 0xFE, 0x4E1F)]
        [DataRow(0x4C, 0x00, 0x4C20)]
        [DataRow(0x4C, 0xF7, 0x4C3E)]
        [DataRow(0x4C, 0xFE, 0x4C3F)]
        [DataRow(0x80, 0x00, 0x5000)]
        [DataRow(0x80, 0xF7, 0x501E)]
        [DataRow(0x80, 0xFE, 0x501F)]
        [DataRow(0x86, 0x00, 0x5600)]
        [DataRow(0x86, 0xF7, 0x561E)]
        [DataRow(0x86, 0xFE, 0x561F)]
        [DataRow(0x8C, 0x00, 0x5420)]
        [DataRow(0x8C, 0xF7, 0x543E)]
        [DataRow(0x8C, 0xFE, 0x543F)]
        public void PIXELAD_WorksAsExpected(int row, int col, int addr)
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0x94           // PIXELAD
            });
            var regs = m.Cpu.Registers;
            regs.D = (byte)row;
            regs.E = (byte)col;

            // --- Act
            m.Run();

            // --- Assert
            regs.HL.ShouldBe((ushort)addr);
            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// PIXELAD: 0xED 0x94
        /// </summary>
        [TestMethod]
        public void PIXELAD_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x94           // PIXELAD
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
        /// LDIX: 0xED 0xA4
        /// </summary>
        [TestMethod]
        public void LDIX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0xA4 // LDIX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x00;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x1001].ShouldBe((byte)0xA5);
            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort)0x1001);
            regs.DE.ShouldBe((ushort)0x1002);
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// LDIX: 0xED 0xA4
        /// </summary>
        [TestMethod]
        public void LDIX_WithNoCopyWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0xA4 // LDIX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0xA5;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x1001].ShouldBe((byte)0x11);
            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort)0x1001);
            regs.DE.ShouldBe((ushort)0x1002);
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(13L);
        }

        /// <summary>
        /// LDIX: 0xED 0xA4
        /// </summary>
        [TestMethod]
        public void LDIX_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0xA4 // LDIX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x00;
            regs.BC = 0x0001;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x1001].ShouldBe((byte)0xA5);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x1001);
            regs.DE.ShouldBe((ushort)0x1002);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// LDIX: 0xED 0xA4
        /// </summary>
        [TestMethod]
        public void LDIX_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA4           // LDIX
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
        /// LDDX: 0xED 0xAC
        /// </summary>
        [TestMethod]
        public void LDDX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0xAC // LDDX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x00;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1001].ShouldBe((byte)0xA5);
            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort)0x0FFF);
            regs.DE.ShouldBe((ushort)0x1000);
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// LDDX: 0xED 0xAC
        /// </summary>
        [TestMethod]
        public void LDDX_WithNoCopyWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0xAC // LDDX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0xA5;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1001].ShouldBe((byte)0x11);
            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort)0x0FFF);
            regs.DE.ShouldBe((ushort)0x1000);
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(13L);
        }

        /// <summary>
        /// LDDX: 0xED 0xAC
        /// </summary>
        [TestMethod]
        public void LDDX_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction, true);
            m.InitCode(new byte[]
            {
                0xED, 0xAC // LDDX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x00;
            regs.BC = 0x0001;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1001].ShouldBe((byte)0xA5);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x0FFF);
            regs.DE.ShouldBe((ushort)0x1000);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// LDDX: 0xED 0xAC
        /// </summary>
        [TestMethod]
        public void LDDX_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xAC           // LDDX
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
        /// LDIRX: 0xED 0xB4
        /// </summary>
        [TestMethod]
        public void LDIRX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd, true);
            m.InitCode(new byte[]
            {
                0xED, 0xB4 // LDIRX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x00;
            regs.BC = 0x0003;
            regs.HL = 0x1001;
            regs.DE = 0x1000;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.HL + 1] = 0xA6;
            m.Memory[regs.HL + 2] = 0xA7;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x1000].ShouldBe((byte)0xA5);
            m.Memory[0x1001].ShouldBe((byte)0xA6);
            m.Memory[0x1002].ShouldBe((byte)0xA7);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x1004);
            regs.DE.ShouldBe((ushort)0x1003);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1000-1002");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(58L);
        }

        /// <summary>
        /// LDIRX: 0xED 0xB4
        /// </summary>
        [TestMethod]
        public void LDIRX_WithNoCopyWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd, true);
            m.InitCode(new byte[]
            {
                0xED, 0xB4 // LDIRX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0xA6;
            regs.BC = 0x0003;
            regs.HL = 0x1001;
            regs.DE = 0x2000;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.HL + 1] = 0xA6;
            m.Memory[regs.HL + 2] = 0xA7;
            m.Memory[regs.DE + 1] = 0xCC;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x2000].ShouldBe((byte)0xA5);
            m.Memory[0x2001].ShouldBe((byte)0xCC);
            m.Memory[0x2002].ShouldBe((byte)0xA7);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x1004);
            regs.DE.ShouldBe((ushort)0x2003);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "2000-2002");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(55L);
        }

        /// <summary>
        /// LDIRX: 0xED 0xB4
        /// </summary>
        [TestMethod]
        public void LDIRX_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xB4           // LDIRX
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
        /// LDDRX: 0xED 0xBC
        /// </summary>
        [TestMethod]
        public void LDDRX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd, true);
            m.InitCode(new byte[]
            {
                0xED, 0xBC // LDDRX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x00;
            regs.BC = 0x0003;
            regs.HL = 0x1002;
            regs.DE = 0x1003;
            m.Memory[regs.HL - 2] = 0xA5;
            m.Memory[regs.HL - 1] = 0xA6;
            m.Memory[regs.HL] = 0xA7;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x1001].ShouldBe((byte)0xA5);
            m.Memory[0x1002].ShouldBe((byte)0xA6);
            m.Memory[0x1003].ShouldBe((byte)0xA7);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x0FFF);
            regs.DE.ShouldBe((ushort)0x1000);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001-1003");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(58L);
        }

        /// <summary>
        /// LDDRX: 0xED 0xBC
        /// </summary>
        [TestMethod]
        public void LDDRX_WithNoCopyWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd, true);
            m.InitCode(new byte[]
            {
                0xED, 0xBC // LDDRX
            });
            var regs = m.Cpu.Registers;
            regs.A = 0xA6;
            regs.BC = 0x0003;
            regs.HL = 0x1002;
            regs.DE = 0x2002;
            m.Memory[regs.HL - 2] = 0xA5;
            m.Memory[regs.HL - 1] = 0xA6;
            m.Memory[regs.HL] = 0xA7;
            m.Memory[regs.DE - 1] = 0xCC;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x2000].ShouldBe((byte)0xA5);
            m.Memory[0x2001].ShouldBe((byte)0xCC);
            m.Memory[0x2002].ShouldBe((byte)0xA7);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x0FFF);
            regs.DE.ShouldBe((ushort)0x1FFF);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "2000-2002");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(55L);
        }

        /// <summary>
        /// LDDRX: 0xED 0xBC
        /// </summary>
        [TestMethod]
        public void LDDRX_DoesNopWithNoExtendedInstructionSet()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xBC           // LDDRX
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
