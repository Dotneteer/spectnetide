using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral
// ReSharper disable InconsistentNaming

namespace Spect.Net.SpectrumEmu.Test.Cpu.IndexedOps
{
    [TestClass]
    public class IxFallbackTests
    {
        /// <summary>
        /// NOP: 0x00
        /// </summary>
        [TestMethod]
        public void NopWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD,
                0x00, // NOP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD BC,NN: 0x01
        /// </summary>
        [TestMethod]
        public void LD_BC_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD,
                0x01, 0x26, 0xA9 // LD BC,A926H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory();

            regs.BC.ShouldBe((ushort)0xA926);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(14L);
        }

        /// <summary>
        /// LD (BC),A: 0x02
        /// </summary>
        [TestMethod]
        public void LD_BCi_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x26, 0xA9, // LD BC,A926H
                0x3E, 0x94,       // LD A,94H
                0xDD,
                0x02              // LD (BC),A
            });

            // --- Act
            var valueBefore = m.Memory[0xA926];
            m.Run();
            var valueAfter = m.Memory[0xA926];

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC, A");
            m.ShouldKeepMemory(except: "A926");

            regs.BC.ShouldBe((ushort)0xA926);
            regs.A.ShouldBe((byte)0x94);
            valueBefore.ShouldBe((byte)0);
            valueAfter.ShouldBe((byte)0x94);
            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(28L);
        }

        /// <summary>
        /// INC BC: 0x03
        /// </summary>
        [TestMethod]
        public void INC_BC_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x26, 0xA9, // LD BC,A926H
                0xDD,
                0x03              // INC BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory();

            regs.BC.ShouldBe((ushort)0xA927);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(20L);
        }

        /// <summary>
        /// INC BC: 0x03
        /// </summary>
        [TestMethod]
        public void INC_BC_WorksAsExpected2()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0xFF, 0xFF, // LD BC,FFFFH
                0xDD,
                0x03              // INC BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory();

            regs.BC.ShouldBe((ushort)0x0000);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(20L);
        }

        /// <summary>
        /// INC B: 0x04
        /// </summary>
        [TestMethod]
        public void INC_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x43, // LD B,43H
                0xDD,
                0x04        // INC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.B.ShouldBe((byte)0x44);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// INC B: 0x04
        /// </summary>
        [TestMethod]
        public void INC_B_SetsZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xFF, // LD B,FFH
                0xDD,
                0x04        // INC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.ZFlag.ShouldBeTrue();

            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// INC B: 0x04
        /// </summary>
        [TestMethod]
        public void INC_B_SetsSignAnOverflowFlags()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x7F, // LD B,7FH
                0xDD,
                0x04        // INC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.SFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.B.ShouldBe((byte)0x80);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// INC B: 0x04
        /// </summary>
        [TestMethod]
        public void INC_B_SetsHalfCarryFlags()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x2F, // LD B,2FH
                0xDD,
                0x04        // INC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeTrue();

            regs.B.ShouldBe((byte)0x30);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// DEC B: 0x05
        /// </summary>
        [TestMethod]
        public void DEC_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x43, // LD B,43H
                0xDD,
                0x05        // DEC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.B.ShouldBe((byte)0x42);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// DEC B: 0x05
        /// </summary>
        [TestMethod]
        public void DEC_B_SetsZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x01, // LD B,01H
                0xDD,
                0x05        // DEC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.ZFlag.ShouldBeTrue();

            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// DEC B: 0x05
        /// </summary>
        [TestMethod]
        public void DEC_B_SetsSignAnOverflowFlags()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x80, // LD B,80H
                0xDD,
                0x05        // DEC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.SFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.B.ShouldBe((byte)0x7F);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// DEC B: 0x05
        /// </summary>
        [TestMethod]
        public void DEC_B_SetsHalfCarryFlags()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x20, // LD B,20H
                0xDD,
                0x05        // DEC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeTrue();

            regs.B.ShouldBe((byte)0x1F);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD B,N: 0x06
        /// </summary>
        [TestMethod]
        public void LD_B_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD,
                0x06, 0x26 // LD B,26H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B");
            m.ShouldKeepMemory();

            regs.B.ShouldBe((byte)0x26);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// RLCA: 0x07
        /// </summary>
        [TestMethod]
        public void RLCA_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x71, // LD A,71H
                0xDD,
                0x07        // RLCA
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeFalse();

            regs.A.ShouldBe((byte)0xE2);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// RLCA: 0x07
        /// </summary>
        [TestMethod]
        public void RLCA_GeneratesCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x80, // LD A,80H
                0xDD,
                0x07        // RLCA
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeTrue();

            regs.A.ShouldBe((byte)0x01);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// EX AF,AF': 0x08
        /// </summary>
        [TestMethod]
        public void EX_AF_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD,
                0x3E, 0x34,  // LD A,34H
                0x08,        // EX AF,AF'
                0x3E, 0x56 , // LD A,56H
                0x08         // EX AF,AF'
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF, AF'");
            m.ShouldKeepMemory();

            regs.A.ShouldBe((byte)0x34);
            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// LD A,(BC): 0x0A
        /// </summary>
        [TestMethod]
        public void LD_A_BCi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x04, 0x00, // LD BC,0004H
                0xDD,
                0x0A              // LD A,(BC)
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC, A");
            m.ShouldKeepMemory();

            regs.A.ShouldBe((byte)0x0A);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(21L);
        }

        /// <summary>
        /// DEC BC: 0x0B
        /// </summary>
        [TestMethod]
        public void DEC_BC_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x26, 0xA9, // LD BC,A926H
                0xDD,
                0x0B              // DEC BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory();

            regs.BC.ShouldBe((ushort)0xA925);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(20L);
        }

        /// <summary>
        /// INC C: 0x0C
        /// </summary>
        [TestMethod]
        public void INC_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0x43, // LD C,43H
                0xDD,
                0x0C        // INC C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "C, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.C.ShouldBe((byte)0x44);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// DEC C: 0x0D
        /// </summary>
        [TestMethod]
        public void DEC_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0x43, // LD C,43H
                0xDD,
                0x0D        // DEC C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "C, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.C.ShouldBe((byte)0x42);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD C,N: 0x0E
        /// </summary>
        [TestMethod]
        public void LD_C_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD,
                0x0E, 0x26 // LD C,26H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "C");
            m.ShouldKeepMemory();

            regs.C.ShouldBe((byte)0x26);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// RRCA: 0x0F
        /// </summary>
        [TestMethod]
        public void RRCA_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x70, // LD A,70H
                0xDD,
                0x0F        // RRCA
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeFalse();

            regs.A.ShouldBe((byte)0x38);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// RRCA: 0x0F
        /// </summary>
        [TestMethod]
        public void RRCA_GeneratesCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x41, // LD A,01H
                0xDD,
                0x0F        // RRCA
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeTrue();

            regs.A.ShouldBe((byte)0xA0);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// DJNZ E: 0x10
        /// </summary>
        [TestMethod]
        public void DJNX_E_WorksWithNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x01, // LD B,01H
                0xDD,
                0x10, 0x02  // DJNZ 02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// DJNZ E: 0x10
        /// </summary>
        [TestMethod]
        public void DJNX_E_WorksWithJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x02, // LD B,02H
                0xDD,
                0x10, 0x02  // DJNZ 02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(24L);
        }

        /// <summary>
        /// LD DE,NN: 0x11
        /// </summary>
        [TestMethod]
        public void LD_DE_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD,
                0x11, 0x26, 0xA9 // LD DE,A926H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.DE.ShouldBe((ushort)0xA926);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(14L);
        }

        /// <summary>
        /// LD (DE),A: 0x12
        /// </summary>
        [TestMethod]
        public void LD_DEi_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x11, 0x26, 0xA9, // LD DE,A926H
                0x3E, 0x94,       // LD A,94H
                0xDD,
                0x12              // LD (DE),A
            });

            // --- Act
            var valueBefore = m.Memory[0xA926];
            m.Run();
            var valueAfter = m.Memory[0xA926];

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "DE, A");
            m.ShouldKeepMemory(except: "A926");

            regs.DE.ShouldBe((ushort)0xA926);
            regs.A.ShouldBe((byte)0x94);
            valueBefore.ShouldBe((byte)0);
            valueAfter.ShouldBe((byte)0x94);
            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(28L);
        }

        /// <summary>
        /// INC DE: 0x13
        /// </summary>
        [TestMethod]
        public void INC_DE_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x11, 0x26, 0xA9, // LD DE,A926H
                0xDD,
                0x13              // INC DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.DE.ShouldBe((ushort)0xA927);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(20L);
        }

        /// <summary>
        /// INC D: 0x14
        /// </summary>
        [TestMethod]
        public void INC_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0x43, // LD B,43H
                0xDD,
                0x14        // INC D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "D, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.D.ShouldBe((byte)0x44);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// DEC D: 0x15
        /// </summary>
        [TestMethod]
        public void DEC_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0x43, // LD D,43H
                0xDD,
                0x15        // DEC D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "D, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.D.ShouldBe((byte)0x42);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD D,N: 0x16
        /// </summary>
        [TestMethod]
        public void LD_D_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD,
                0x16, 0x26 // LD B,26H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "D");
            m.ShouldKeepMemory();

            regs.D.ShouldBe((byte)0x26);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// RLA: 0x17
        /// </summary>
        [TestMethod]
        public void RLA_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x81, // LD A,81H
                0x17        // RLA
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeTrue();

            regs.A.ShouldBe((byte)0x02);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// RLA: 0x17
        /// </summary>
        [TestMethod]
        public void RLA_UsesCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x20, // LD A,20H
                0x37,       // SCF
                0xDD,
                0x17        // RLA
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeFalse();

            regs.A.ShouldBe((byte)0x41);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// JR E: 0x18
        /// </summary>
        [TestMethod]
        public void JR_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x20, // LD A,20H
                0xDD,
                0x18, 0x20  // JR 20H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();
            regs.PC.ShouldBe((ushort)0x0025);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// LD A,(DE): 0x1A
        /// </summary>
        [TestMethod]
        public void LD_A_DEi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x11, 0x04, 0x00, // LD DE,0004H
                0xDD,
                0x1A              // LD A,(DE)
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "DE, A");
            m.ShouldKeepMemory();

            regs.A.ShouldBe((byte)0x1A);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(21L);
        }

        /// <summary>
        /// DEC DE: 0x1B
        /// </summary>
        [TestMethod]
        public void DEC_DE_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x11, 0x26, 0xA9, // LD DE,A926H
                0xDD,
                0x1B              // DEC DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.DE.ShouldBe((ushort)0xA925);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(20L);
        }

        /// <summary>
        /// INC E: 0x1C
        /// </summary>
        [TestMethod]
        public void INC_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0x43, // LD E,43H
                0xDD,
                0x1C        // INC E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "E, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.E.ShouldBe((byte)0x44);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// DEC E: 0x1D
        /// </summary>
        [TestMethod]
        public void DEC_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0x43, // LD E,43H
                0xDD,
                0x1D        // DEC E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "E, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.E.ShouldBe((byte)0x42);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD E,N: 0x1E
        /// </summary>
        [TestMethod]
        public void LD_E_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD,
                0x1E, 0x26 // LD E,26H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "E");
            m.ShouldKeepMemory();

            regs.E.ShouldBe((byte)0x26);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// RRA: 0x1F
        /// </summary>
        [TestMethod]
        public void RRA_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x81, // LD A,81H
                0xDD,
                0x1F        // RRA
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeTrue();

            regs.A.ShouldBe((byte)0x40);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// RRA: 0x1F
        /// </summary>
        [TestMethod]
        public void RRA_UsesCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x20, // LD A,20H
                0x37,       // SCF
                0xDD,
                0x1F        // RRA
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeFalse();

            regs.A.ShouldBe((byte)0x90);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// JR NZ,E: 0x20
        /// </summary>
        [TestMethod]
        public void JR_NZ_E_WorksWithNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x01, // LD A,01H
                0x3D,       // DEC A 
                0xDD,
                0x20, 0x02  // JR NZ,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// JR NZ,E: 0x20
        /// </summary>
        [TestMethod]
        public void JR_NZ_E_WorksWithJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x02, // LD A,02H
                0x3D,       // DEC A 
                0xDD,
                0x20, 0x02  // JR NZ,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(27L);
        }

        /// <summary>
        /// DAA: 0x27
        /// </summary>
        [TestMethod]
        public void DAA_WorksAsExpected()
        {
            // --- Arrange
            var samples = new[]
            {
                new DaaSample(0x99, false, false, false, 0x998C),
                new DaaSample(0x99, true, false, false, 0x9F8C),
                new DaaSample(0x7A, false, false, false, 0x8090),
                new DaaSample(0x7A, true, false, false, 0x8090),
                new DaaSample(0xA9, false, false, false, 0x090D),
                new DaaSample(0x87, false, false, true, 0xE7A5),
                new DaaSample(0x87, true, false, true, 0xEDAD),
                new DaaSample(0x1B, false, false, true, 0x8195),
                new DaaSample(0x1B, true, false, true, 0x8195),
                new DaaSample(0xAA, false, false, false, 0x1011),
                new DaaSample(0xAA, true, false, false, 0x1011),
                new DaaSample(0xC6, true, false, false, 0x2C29)
            };

            // --- Act
            foreach (var sample in samples)
            {
                var m = new Z80TestMachine(RunMode.UntilEnd);
                m.InitCode(new byte[]
                {
                    0xDD,
                    0x27  // DAA
                });
                m.Cpu.Registers.A = sample.A;
                m.Cpu.Registers.F = (byte)((sample.H ? FlagsSetMask.H : 0)
                                           | (sample.N ? FlagsSetMask.N : 0)
                                           | (sample.C ? FlagsSetMask.C : 0));

                // --- Act
                m.Run();

                // --- Assert
                var regs = m.Cpu.Registers;

                m.ShouldKeepRegisters(except: "AF");
                m.ShouldKeepMemory();

                regs.AF.ShouldBe(sample.AF);
                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// JR Z,E: 0x28
        /// </summary>
        [TestMethod]
        public void JR_Z_E_WorksWithNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x02, // LD A,02H
                0x3D,       // DEC A 
                0xDD,
                0x28, 0x02  // JR Z,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// JR Z,E: 0x28
        /// </summary>
        [TestMethod]
        public void JR_Z_E_WorksWithJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x01, // LD A,01H
                0x3D,       // DEC A 
                0xDD,
                0x28, 0x02  // JR Z,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(27L);
        }

        /// <summary>
        /// CPL: 0x2F
        /// </summary>
        [TestMethod]
        public void CPL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x81, // LD A,81H
                0xDD,
                0x2F        // CPL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            m.ShouldKeepCFlag();
            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();

            regs.A.ShouldBe((byte)0x7E);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// JR NC,E: 0x30
        /// </summary>
        [TestMethod]
        public void JR_NC_E_WorksWithNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37,       // SCF 
                0xDD,
                0x30, 0x02  // JR NC,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// JR NC,E: 0x30
        /// </summary>
        [TestMethod]
        public void JR_NC_E_WorksWithJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37,       // SCF
                0x3F,       // CCF 
                0xDD,
                0x30, 0x02  // JR NC,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(24L);
        }

        /// <summary>
        /// LD SP,NN: 0x31
        /// </summary>
        [TestMethod]
        public void LD_SP_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD,
                0x31, 0x26, 0xA9 // LD SP,A926H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory();

            regs.SP.ShouldBe((ushort)0xA926);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(14L);
        }

        /// <summary>
        /// LD (NN),A: 0x32
        /// </summary>
        [TestMethod]
        public void LD_NNi_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xA9,       // LD A,A9H
                0xDD,
                0x32, 0x00, 0x10  // LD (1000H),A
            });

            // --- Act
            var before = m.Memory[0x1000];
            m.Run();
            var after = m.Memory[0x1000];
            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory(except: "1000");

            before.ShouldBe((byte)0x00);
            after.ShouldBe((byte)0xA9);

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(24L);
        }

        /// <summary>
        /// INC SP: 0x33
        /// </summary>
        [TestMethod]
        public void INC_SP_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x31, 0x26, 0xA9, // LD SP,A926H
                0xDD,
                0x33              // INC SP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory();

            regs.SP.ShouldBe((ushort)0xA927);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(20L);
        }

        /// <summary>
        /// SCF: 0x37
        /// </summary>
        [TestMethod]
        public void SCF_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD,
                0x37 // SCF
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            regs.CFlag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// JR C,E: 0x38
        /// </summary>
        [TestMethod]
        public void JR_C_E_WorksWithNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37,       // SCF
                0x3F,       // CCF
                0xDD,
                0x38, 0x02  // JR C,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// JR C,E: 0x38
        /// </summary>
        [TestMethod]
        public void JR_C_E_WorksWithJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37,       // SCF 
                0xDD,
                0x38, 0x02  // JR C,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(20L);
        }

        /// <summary>
        /// LD A,(NN): 0x3A
        /// </summary>
        [TestMethod]
        public void LD_A_NNi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD,
                0x3A, 0x00, 0x10 // LD A,(1000H)
            });
            m.Memory[0x1000] = 0x34;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();

            regs.A.ShouldBe((byte)0x34);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(17L);
        }

        /// <summary>
        /// DEC SP: 0x3B
        /// </summary>
        [TestMethod]
        public void DEC_SP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x31, 0x26, 0xA9, // LD SP,A926H
                0xDD,
                0x3B              // DEC SP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory();

            regs.SP.ShouldBe((ushort)0xA925);
            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(20L);
        }

        /// <summary>
        /// INC A: 0x3C
        /// </summary>
        [TestMethod]
        public void INC_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x43, // LD L,43H
                0xDD,
                0x3C        // INC A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.A.ShouldBe((byte)0x44);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// DEC A: 0x3D
        /// </summary>
        [TestMethod]
        public void DEC_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x43, // LD A,43H
                0xDD,
                0x3D        // DEC A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.A.ShouldBe((byte)0x42);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD A,N: 0x3E
        /// </summary>
        [TestMethod]
        public void LD_L_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD,
                0x3E, 0x26 // LD A,26H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();

            regs.A.ShouldBe((byte)0x26);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// CCF: 0x3F
        /// </summary>
        [TestMethod]
        public void CCF_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37, // SCF
                0xDD,
                0x3F  // CCF
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();
            regs.CFlag.ShouldBeFalse();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(12L);
        }

        /// <summary>
        /// LD B,C: 0x41
        /// </summary>
        [TestMethod]
        public void LD_B_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0e, 0xB9, // LD C,B9H
                0xDD,
                0x41        // LD B,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD B,D: 0x42
        /// </summary>
        [TestMethod]
        public void LD_B_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0xB9, // LD D,B9H
                0xDD,
                0x42        // LD B,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD B,E: 0x43
        /// </summary>
        [TestMethod]
        public void LD_B_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0xB9, // LD E,B9H
                0xDD,
                0x43        // LD E,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD B,A: 0x47
        /// </summary>
        [TestMethod]
        public void LD_B_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xB9, // LD A,B9H
                0xDD,
                0x47        // LD B,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD C,B: 0x48
        /// </summary>
        [TestMethod]
        public void LD_C_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xB9, // LD B,B9H
                0xDD,
                0x48        // LD C,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD C,D: 0x4A
        /// </summary>
        [TestMethod]
        public void LD_C_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0xB9, // LD D,B9H
                0xDD,
                0x4A        // LD C,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD C,E: 0x4B
        /// </summary>
        [TestMethod]
        public void LD_C_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0xB9, // LD E,B9H
                0xDD,
                0x4B        // LD C,E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD C,A: 0x4F
        /// </summary>
        [TestMethod]
        public void LD_C_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xB9, // LD A,B9H
                0xDD,
                0x4F        // LD C,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD D,B: 0x50
        /// </summary>
        [TestMethod]
        public void LD_D_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xB9, // LD B,B9H
                0xDD,
                0x50        // LD D,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD D,C: 0x51
        /// </summary>
        [TestMethod]
        public void LD_D_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0xB9, // LD C,B9H
                0xDD,
                0x51        // LD D,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD D,E: 0x53
        /// </summary>
        [TestMethod]
        public void LD_D_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0xB9, // LD E,B9H
                0xDD,
                0x53        // LD D,E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "D, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD D,A: 0x57
        /// </summary>
        [TestMethod]
        public void LD_D_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xB9, // LD A,B9H
                0xDD,
                0x57        // LD D,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "D, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD E,B: 0x58
        /// </summary>
        [TestMethod]
        public void LD_E_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xB9, // LD B,B9H
                0xDD,
                0x58        // LD E,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "E, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD E,C: 0x59
        /// </summary>
        [TestMethod]
        public void LD_E_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0xB9, // LD C,B9H
                0xDD,
                0x59        // LD E,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "E, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD E,D: 0x5A
        /// </summary>
        [TestMethod]
        public void LD_E_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0xB9, // LD D,B9H
                0xDD,
                0x5A        // LD E,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD E,A: 0x5F
        /// </summary>
        [TestMethod]
        public void LD_E_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xB9, // LD A,B9H
                0xDD,
                0x5F        // LD E,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "E, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// HALT: 0x76
        /// </summary>
        [TestMethod]
        public void HALTWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0xDD,
                0x76        // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            (m.Cpu.StateFlags & Z80StateFlags.Halted).ShouldBe(Z80StateFlags.Halted);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0001);
            m.Cpu.Tacts.ShouldBe(8L);
        }


        /// <summary>
        /// LD A,B: 0x78
        /// </summary>
        [TestMethod]
        public void LD_A_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xB9, // LD B,B9H
                0xDD,
                0x78        // LD A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "A, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD A,C: 0x79
        /// </summary>
        [TestMethod]
        public void LD_A_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0xB9, // LD C,B9H
                0xDD,
                0x79        // LD A,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "A, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// LD A,D: 0x7A
        /// </summary>
        [TestMethod]
        public void LD_A_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0xB9, // LD D,B9H
                0xDD,
                0x7A        // LD A,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "A, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }


        /// <summary>
        /// LD A,E: 0x7B
        /// </summary>
        [TestMethod]
        public void LD_A_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0xB9, // LD E,B9H
                0xDD,
                0x7B        // LD A,E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "A, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// ADD A,B: 0x80
        /// </summary>
        [TestMethod]
        public void ADD_A_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x06, 0x24, // LD B,24H
                0xDD,
                0x80        // ADD A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x36);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADD A,B: 0x80
        /// </summary>
        [TestMethod]
        public void ADD_A_B_HandlesCarryFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF0, // LD A,F0H
                0x06, 0xF0, // LD B,F0H
                0xDD,
                0x80        // ADD A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xE0);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADD A,B: 0x80
        /// </summary>
        [TestMethod]
        public void ADD_A_B_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x82, // LD A,82H
                0x06, 0x7E, // LD B,7EH
                0xDD,
                0x80        // ADD A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x0);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADD A,B: 0x80
        /// </summary>
        [TestMethod]
        public void ADD_A_B_HandlesSignFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x44, // LD A,44H
                0x06, 0x42, // LD B,42H
                0xDD,
                0x80        // ADD A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x86);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADD A,C: 0x81
        /// </summary>
        [TestMethod]
        public void ADD_A_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x0E, 0x24, // LD C,24H
                0xDD,
                0x81        // ADD A,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x36);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADD A,D: 0x82
        /// </summary>
        [TestMethod]
        public void ADD_A_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x16, 0x24, // LD D,24H
                0xDD,
                0x82        // ADD A,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x36);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADD A,E: 0x83
        /// </summary>
        [TestMethod]
        public void ADD_A_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x1E, 0x24, // LD E,24H
                0xDD,
                0x83        // ADD A,E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x36);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADD A,A: 0x87
        /// </summary>
        [TestMethod]
        public void ADD_A_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD,
                0x87        // ADD A,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x24);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }


        /// <summary>
        /// ADC A,B: 0x88
        /// </summary>
        [TestMethod]
        public void ADC_A_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x06, 0x24, // LD B,24H
                0xDD,
                0x88        // ADC A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x36);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADC A,B: 0x88
        /// </summary>
        [TestMethod]
        public void ADC_A_B_HandlesCarryFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF0, // LD A,F0H
                0x06, 0xF0, // LD B,F0H
                0xDD,
                0x88        // ADC A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xE0);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADC A,B: 0x88
        /// </summary>
        [TestMethod]
        public void ADC_A_B_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x82, // LD A,82H
                0x06, 0x7E, // LD B,7EH
                0xDD,
                0x88        // ADC A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x0);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADC A,B: 0x88
        /// </summary>
        [TestMethod]
        public void ADC_A_B_HandlesSignFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x44, // LD A,44H
                0x06, 0x42, // LD B,42H
                0xDD,
                0x88        // ADC A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x86);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// ADC A,B: 0x88
        /// </summary>
        [TestMethod]
        public void ADC_A_B_WithCarryWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x06, 0x24, // LD B,24H
                0x37,       // SCF
                0xDD,
                0x88        // ADC A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x37);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// ADC A,B: 0x88
        /// </summary>
        [TestMethod]
        public void ADC_A_B_WithCarryHandlesCarryFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF0, // LD A,F0H
                0x06, 0xF0, // LD B,F0H
                0x37,       // SCF
                0xDD,
                0x88        // ADC A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xE1);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// ADC A,B: 0x88
        /// </summary>
        [TestMethod]
        public void ADC_A_B_WithCarryHandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x82, // LD A,82H
                0x06, 0x7D, // LD B,7DH
                0x37,       // SCF
                0xDD,
                0x88        // ADC A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x0);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// ADC A,B: 0x88
        /// </summary>
        [TestMethod]
        public void ADC_A_B_WithCarryHandlesSignFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x44, // LD A,44H
                0x06, 0x42, // LD B,42H
                0x37,       // SCF
                0xDD,
                0x88        // ADC A,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x87);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// ADC A,C: 0x89
        /// </summary>
        [TestMethod]
        public void ADC_A_C_WithCarryWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x0E, 0x24, // LD C,24H
                0x37,       // SCF
                0xDD,
                0x89        // ADC A,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x37);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// ADC A,D: 0x8A
        /// </summary>
        [TestMethod]
        public void ADC_A_D_WithCarryWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x16, 0x24, // LD D,24H
                0x37,       // SCF
                0xDD,
                0x8A        // ADC A,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x37);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// ADC A,E: 0x8B
        /// </summary>
        [TestMethod]
        public void ADC_A_E_WithCarryWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x1E, 0x24, // LD E,24H
                0x37,       // SCF
                0xDD,
                0x8B        // ADC A,E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x37);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// ADC A,A: 0x8F
        /// </summary>
        [TestMethod]
        public void ADC_A_A_WithCarryWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x37,       // SCF
                0xDD,
                0x8F        // ADC A,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x25);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// SUB B: 0x90
        /// </summary>
        [TestMethod]
        public void SUB_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x06, 0x24, // LD B,24H
                0xDD,
                0x90        // SUB B
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

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// SUB B: 0x90
        /// </summary>
        [TestMethod]
        public void SUB_B_HandlesCarryFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x40, // LD A,40H
                0x06, 0x60, // LD B,60H
                0xDD,
                0x90        // SUB B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xE0);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// SUB B: 0x90
        /// </summary>
        [TestMethod]
        public void SUB_B_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x40, // LD A,40H
                0x06, 0x40, // LD B,40H
                0xDD,
                0x90        // SUB B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x00);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// SUB B: 0x90
        /// </summary>
        [TestMethod]
        public void SUB_B_HandlesHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x41, // LD A,41H
                0x06, 0x43, // LD B,43H
                0xDD,
                0x90        // SUB B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xFE);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// SUB B: 0x90
        /// </summary>
        [TestMethod]
        public void SUB_B_HandlesPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x61, // LD A,61H
                0x06, 0xB3, // LD B,B3H
                0xDD,
                0x90        // SUB B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xAE);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// SUB C: 0x91
        /// </summary>
        [TestMethod]
        public void SUB_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x0E, 0x24, // LD C,24H
                0xDD,
                0x91        // SUB C
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
            m.ShouldKeepRegisters(except: "AF, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// SUB D: 0x92
        /// </summary>
        [TestMethod]
        public void SUB_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x16, 0x24, // LD D,24H
                0xDD,
                0x92        // SUB D
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
            m.ShouldKeepRegisters(except: "AF, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// SUB E: 0x93
        /// </summary>
        [TestMethod]
        public void SUB_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x1E, 0x24, // LD E,24H
                0xDD,
                0x93        // SUB E
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
            m.ShouldKeepRegisters(except: "AF, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// SUB A: 0x97
        /// </summary>
        [TestMethod]
        public void SUB_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0xDD,
                0x97        // SUB A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x00);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// SBC B: 0x98
        /// </summary>
        [TestMethod]
        public void SBC_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x06, 0x24, // LD B,24H
                0x37,       // SCF
                0xDD,
                0x98        // SBC B
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
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// SBC B: 0x98
        /// </summary>
        [TestMethod]
        public void SBC_B_HandlesCarryFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x40, // LD A,40H
                0x06, 0x60, // LD B,60H
                0x37,       // SCF
                0xDD,
                0x98        // SBC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xDF);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// SBC B: 0x98
        /// </summary>
        [TestMethod]
        public void SBC_B_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x40, // LD A,40H
                0x06, 0x3F, // LD B,3FH
                0x37,       // SCF
                0xDD,
                0x98        // SBC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x00);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// SBC B: 0x98
        /// </summary>
        [TestMethod]
        public void SBC_B_HandlesHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x41, // LD A,41H
                0x06, 0x43, // LD B,43H
                0x37,       // SCF
                0xDD,
                0x98        // SBC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xFD);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// SBC B: 0x98
        /// </summary>
        [TestMethod]
        public void SBC_B_HandlesPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x61, // LD A,61H
                0x06, 0xB3, // LD B,B3H
                0x37,       // SCF
                0xDD,
                0x98        // SBC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xAD);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// SBC C: 0x99
        /// </summary>
        [TestMethod]
        public void SBC_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x0E, 0x24, // LD C,24H
                0x37,       // SCF
                0xDD,
                0x99        // SBC C
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
            m.ShouldKeepRegisters(except: "AF, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// SBC D: 0x9A
        /// </summary>
        [TestMethod]
        public void SBC_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x16, 0x24, // LD D,24H
                0x37,       // SCF
                0xDD,
                0x9A        // SBC D
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
            m.ShouldKeepRegisters(except: "AF, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// SBC E: 0x9B
        /// </summary>
        [TestMethod]
        public void SBC_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x1E, 0x24, // LD E,24H
                0x37,       // SCF
                0xDD,
                0x9B        // SBC E
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
            m.ShouldKeepRegisters(except: "AF, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// SBC A: 0x9F
        /// </summary>
        [TestMethod]
        public void SBC_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36, // LD A,36H
                0x37,       // SCF
                0xDD,
                0x9F        // SBC A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xFF);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// AND B: 0xA0
        /// </summary>
        [TestMethod]
        public void AND_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x06, 0x23, // LD B,23H
                0xDD,
                0xA0        // AND B
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
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// AND B: 0xA0
        /// </summary>
        [TestMethod]
        public void AND_B_HandlesSignFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF2, // LD A,F2H
                0x06, 0xF3, // LD B,F3H
                0xDD,
                0xA0        // AND B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xF2);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// AND B: 0xA0
        /// </summary>
        [TestMethod]
        public void AND_B_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xC3, // LD A,C3H
                0x06, 0x3C, // LD B,37H
                0xDD,
                0xA0        // AND B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x00);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// AND B: 0xA0
        /// </summary>
        [TestMethod]
        public void AND_B_HandlesPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x33, // LD A,33H
                0x06, 0x22, // LD B,22H
                0xDD,
                0xA0        // AND B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x22);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// AND C: 0xA1
        /// </summary>
        [TestMethod]
        public void AND_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x0E, 0x23, // LD C,23H
                0xDD,
                0xA1        // AND C
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
            m.ShouldKeepRegisters(except: "AF, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// AND D: 0xA2
        /// </summary>
        [TestMethod]
        public void AND_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x16, 0x23, // LD D,23H
                0xDD,
                0xA2        // AND D
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
            m.ShouldKeepRegisters(except: "AF, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// AND E: 0xA3
        /// </summary>
        [TestMethod]
        public void AND_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x1E, 0x23, // LD E,23H
                0xDD,
                0xA3        // AND E
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
            m.ShouldKeepRegisters(except: "AF, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// AND A: 0xA7
        /// </summary>
        [TestMethod]
        public void AND_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD,
                0xA7        // AND A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x12);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// XOR B: 0xA8
        /// </summary>
        [TestMethod]
        public void XOR_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x06, 0x23, // LD B,23H
                0xDD,
                0xA8        // XOR B
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
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// XOR B: 0xA0
        /// </summary>
        [TestMethod]
        public void XOR_B_HandlesSignFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF2, // LD A,F2H
                0x06, 0x03, // LD B,F3H
                0xDD,
                0xA8        // XOR B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xF1);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// XOR B: 0xA8
        /// </summary>
        [TestMethod]
        public void XOR_B_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x43, // LD A,C3H
                0x06, 0x43, // LD B,C3H
                0xDD,
                0xA8        // XOR B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x00);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// XOR B: 0xA8
        /// </summary>
        [TestMethod]
        public void XOR_B_HandlesPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x33, // LD A,33H
                0x06, 0x22, // LD B,22H
                0xDD,
                0xA8        // XOR B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x11);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// XOR C: 0xA9
        /// </summary>
        [TestMethod]
        public void XOR_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x0E, 0x23, // LD C,23H
                0xDD,
                0xA9        // XOR C
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
            m.ShouldKeepRegisters(except: "AF, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// XOR D: 0xAA
        /// </summary>
        [TestMethod]
        public void XOR_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x16, 0x23, // LD D,23H
                0xDD,
                0xAA        // XOR D
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
            m.ShouldKeepRegisters(except: "AF, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// XOR E: 0xAB
        /// </summary>
        [TestMethod]
        public void XOR_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x1E, 0x23, // LD E,23H
                0xDD,
                0xAB        // XOR E
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
            m.ShouldKeepRegisters(except: "AF, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// XOR A: 0xAF
        /// </summary>
        [TestMethod]
        public void XOR_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD,
                0xAF        // XOR A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x00);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// OR B: 0xB0
        /// </summary>
        [TestMethod]
        public void OR_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x52, // LD A,52H
                0x06, 0x23, // LD B,23H
                0xDD,
                0xB0        // OR B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x73);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// OR B: 0xB0
        /// </summary>
        [TestMethod]
        public void OR_B_WorksWithSignFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x82, // LD A,82H
                0x06, 0x22, // LD B,22H
                0xDD,
                0xB0        // OR B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0xA2);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// OR B: 0xB0
        /// </summary>
        [TestMethod]
        public void OR_B_WorksWithZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x00, // LD A,00H
                0x06, 0x00, // LD B,00H
                0xDD,
                0xB0        // OR B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x00);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// OR B: 0xB0
        /// </summary>
        [TestMethod]
        public void OR_B_WorksWithPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x32, // LD A,32H
                0x06, 0x11, // LD B,11H
                0xDD,
                0xB0        // OR B
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
            m.ShouldKeepRegisters(except: "AF, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// OR C: 0xB1
        /// </summary>
        [TestMethod]
        public void OR_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x52, // LD A,52H
                0x0E, 0x23, // LD C,23H
                0xDD,
                0xB1        // OR C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x73);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// OR D: 0xB2
        /// </summary>
        [TestMethod]
        public void OR_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x52, // LD A,52H
                0x16, 0x23, // LD D,23H
                0xDD,
                0xB2        // OR D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x73);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// OR E: 0xB3
        /// </summary>
        [TestMethod]
        public void OR_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x52, // LD A,52H
                0x1E, 0x23, // LD E,23H
                0xDD,
                0xB3        // OR E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x73);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// OR A: 0xB7
        /// </summary>

        [TestMethod]
        public void OR_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x52, // LD A,52H
                0xDD,
                0xB7        // OR A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x52);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP B: 0xB8
        /// </summary>
        [TestMethod]
        public void CP_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x24, // LD B,24H
                0xDD,
                0xB8        // CP B
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
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP B: 0xB8
        /// </summary>
        [TestMethod]
        public void CP_B_HandlesCarryFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x60, // LD B,60H
                0xDD,
                0xB8        // CP B
            });
            m.Cpu.Registers.A = 0x40;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP B: 0xB8
        /// </summary>
        [TestMethod]
        public void CP_B_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x40, // LD B,40H
                0xDD,
                0xB8        // CP B
            });
            m.Cpu.Registers.A = 0x40;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP B: 0xB8
        /// </summary>
        [TestMethod]
        public void CP_B_HandlesHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x43, // LD B,43H
                0xDD,
                0xB8        // CP B
            });
            m.Cpu.Registers.A = 0x41;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP B: 0xB8
        /// </summary>
        [TestMethod]
        public void CP_B_HandlesHPlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xB3, // LD B,B3H
                0xDD,
                0xB8        // CP B
            });
            m.Cpu.Registers.A = 0x61;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP C: 0xB9
        /// </summary>
        [TestMethod]
        public void CP_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0x24, // LD C,24H
                0xDD,
                0xB9        // CP C
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
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP C: 0xB9
        /// </summary>
        [TestMethod]
        public void CP_C_HandlesCarryFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0x60, // LD C,60H
                0xDD,
                0xB9        // CP C
            });
            m.Cpu.Registers.A = 0x40;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP C: 0xB9
        /// </summary>
        [TestMethod]
        public void CP_C_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0x40, // LD C,40H
                0xDD,
                0xB9        // CP B
            });
            m.Cpu.Registers.A = 0x40;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP C: 0xB9
        /// </summary>
        [TestMethod]
        public void CP_C_HandlesHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0x43, // LD C,43H
                0xDD,
                0xB9        // CP C
            });
            m.Cpu.Registers.A = 0x41;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP C: 0xB9
        /// </summary>
        [TestMethod]
        public void CP_C_HandlesHPlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0xB3, // LD C,B3H
                0xDD,
                0xB9        // CP C
            });
            m.Cpu.Registers.A = 0x61;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP D: 0xBA
        /// </summary>
        [TestMethod]
        public void CP_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0x24, // LD D,24H
                0xDD,
                0xBA        // CP D
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
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP D: 0xBA
        /// </summary>
        [TestMethod]
        public void CP_D_HandlesCarryFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0x60, // LD D,60H
                0xDD,
                0xBA        // CP D
            });
            m.Cpu.Registers.A = 0x40;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP D: 0xBA
        /// </summary>
        [TestMethod]
        public void CP_D_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0x40, // LD D,40H
                0xDD,
                0xBA        // CP D
            });
            m.Cpu.Registers.A = 0x40;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP D: 0xBA
        /// </summary>
        [TestMethod]
        public void CP_D_HandlesHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0x43, // LD D,43H
                0xDD,
                0xBA        // CP D
            });
            m.Cpu.Registers.A = 0x41;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP D: 0xBA
        /// </summary>
        [TestMethod]
        public void CP_D_HandlesHPlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0xB3, // LD D,B3H
                0xDD,
                0xBA        // CP D
            });
            m.Cpu.Registers.A = 0x61;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP E: 0xBB
        /// </summary>
        [TestMethod]
        public void CP_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0x24, // LD E,24H
                0xDD,
                0xBB        // CP E
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
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP E: 0xBB
        /// </summary>
        [TestMethod]
        public void CP_E_HandlesCarryFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0x60, // LD E,60H
                0xDD,
                0xBB        // CP E
            });
            m.Cpu.Registers.A = 0x40;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP E: 0xBB
        /// </summary>
        [TestMethod]
        public void CP_E_HandlesZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0x40, // LD E,40H
                0xDD,
                0xBB        // CP E
            });
            m.Cpu.Registers.A = 0x40;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP E: 0xBB
        /// </summary>
        [TestMethod]
        public void CP_E_HandlesHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0x43, // LD E,43H
                0xDD,
                0xBB        // CP E
            });
            m.Cpu.Registers.A = 0x41;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP E: 0xBB
        /// </summary>
        [TestMethod]
        public void CP_E_HandlesHPlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0xB3, // LD E,B3H
                0xDD,
                0xBB        // CP E
            });
            m.Cpu.Registers.A = 0x61;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// CP A: 0xBF
        /// </summary>
        [TestMethod]
        public void CP_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD,
                0xBF        // CP A
            });
            m.Cpu.Registers.A = 0x36;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(47L);
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
                0xDD,
                0xC1              // POP BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.BC.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "HL, BC");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(35L);
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
                0xDD,
                0xC2, 0x08, 0x00, // JP NZ,0008H
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
            m.Cpu.Tacts.ShouldBe(36L);
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
                0xDD,
                0xC3, 0x07, 0x00, // JP 0006H
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
                0xDD,
                0xC4, 0x08, 0x00, // CALL NZ,0008H
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
            m.Cpu.Tacts.ShouldBe(53L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(35L);
        }

        /// <summary>
        /// ADD A,N: 0xC6
        /// </summary>
        [TestMethod]
        public void ADD_A_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD,
                0xC6, 0x24 // ADD,24H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x36);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18L);
        }

        /// <summary>
        /// RST 00H: 0xC7
        /// </summary>
        [TestMethod]
        public void RST_0_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD,
                0xC7        // RST 0
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x12);

            regs.SP.ShouldBe((ushort)0xFFFE);
            regs.PC.ShouldBe((ushort)0);
            m.Memory[0xFFFE].ShouldBe((byte)0x04);
            m.Memory[0xFFFF].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(22L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(47L);
        }

        /// <summary>
        /// RET: 0xC9
        /// </summary>
        [TestMethod]
        public void RET_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xDD,
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(42L);
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
                0xDD,
                0xCA, 0x08, 0x00, // JP Z,0008H
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
            m.Cpu.Tacts.ShouldBe(36L);
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
                0xDD,
                0xCC, 0x08, 0x00, // CALL Z,0008H
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
            m.Cpu.Tacts.ShouldBe(53L);
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
                0xDD,
                0xCD, 0x07, 0x00, // CALL 0007H
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
            m.Cpu.Tacts.ShouldBe(49L);
        }

        /// <summary>
        /// ADC A,N: 0xCE
        /// </summary>
        [TestMethod]
        public void ADC_A_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x37,       // SCF
                0xDD,
                0xCE, 0x24  // ADC,24H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x37);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// RST 08h: 0xCF
        /// </summary>
        [TestMethod]
        public void RST_8_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD,
                0xCF        // RST 8
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x12);

            regs.SP.ShouldBe((ushort)0xFFFE);
            regs.PC.ShouldBe((ushort)8);
            m.Memory[0xFFFE].ShouldBe((byte)0x04);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(22L);
        }

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
                0xA7,             // AND A
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(47L);
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
                0xDD,
                0xD1              // POP DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.DE.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "HL, DE");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(35L);
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
                0xA7,             // AND A
                0xDD,
                0xD2, 0x08, 0x00, // JP NC,0007H
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
            m.Cpu.Tacts.ShouldBe(36L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(22L);
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
                0xA7,             // AND A
                0xDD,
                0xD4, 0x08, 0x00, // CALL NC,0008H
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
            m.Cpu.Tacts.ShouldBe(53L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(35L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18L);
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
                0xDD,
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
            m.Memory[0xFFFE].ShouldBe((byte)0x04);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(22L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(47L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
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
                0xDD,
                0xDA, 0x08, 0x00, // JP C,0008H
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
            m.Cpu.Tacts.ShouldBe(36L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(22L);
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
                0xDD,
                0xDC, 0x08, 0x00, // CALL C,0008H
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
            m.Cpu.Tacts.ShouldBe(53L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
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
                0xDD,
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
            m.Memory[0xFFFE].ShouldBe((byte)0x04);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(22L);
        }

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
                0xDD,
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
            m.Cpu.Tacts.ShouldBe(47L);
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
                0xDD,
                0xE2, 0x08, 0x00, // JP PO,0008H
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
            m.Cpu.Tacts.ShouldBe(36L);
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
                0xDD,
                0xE4, 0x08, 0x00, // CALL PO,0008H
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
            m.Cpu.Tacts.ShouldBe(53L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18L);
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
                0xDD,
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
            m.Memory[0xFFFE].ShouldBe((byte)0x04);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(22L);
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
                0xDD,
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
            m.Cpu.Tacts.ShouldBe(47L);
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
                0xDD,
                0xEA, 0x08, 0x00, // JP PE,0008H
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
            m.Cpu.Tacts.ShouldBe(36L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(28L);
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
                0xDD,
                0xEC, 0x08, 0x00, // CALL PE,0008H
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
            m.Cpu.Tacts.ShouldBe(53L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18L);
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
                0xDD,
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
            m.Memory[0xFFFE].ShouldBe((byte)0x04);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(22L);
        }

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
                0xDD,
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
            m.Cpu.Tacts.ShouldBe(47L);
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
                0xDD,
                0xF1              // POP AF
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.AF.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "AF, BC");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(35L);
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
                0xDD,
                0xF2, 0x08, 0x00, // JP P,0008H
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
            m.Cpu.Tacts.ShouldBe(36L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
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
                0xDD,
                0xF4, 0x08, 0x00, // CALL P,0008H
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
            m.Cpu.Tacts.ShouldBe(53L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(25L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18L);
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
                0xDD,
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
            m.Memory[0xFFFE].ShouldBe((byte)0x04);
            m.Memory[0xFFFF].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(22L);
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
                0xDD,
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
            m.Cpu.Tacts.ShouldBe(47L);
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
                0xDD,
                0xFA, 0x08, 0x00, // JP M,0008H
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
            m.Cpu.Tacts.ShouldBe(36L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
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
                0xDD,
                0xFC, 0x08, 0x00, // CALL M,0008H
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
            m.Cpu.Tacts.ShouldBe(53L);
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
                0xDD,
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

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
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
                0xDD,
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
            m.Memory[0xFFFE].ShouldBe((byte)0x04);
            m.Memory[0xFFFf].ShouldBe((byte)0x00);

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            m.Cpu.Tacts.ShouldBe(22L);
        }

        private class DaaSample
        {
            public readonly byte A;
            public readonly bool H;
            public readonly bool N;
            public readonly bool C;
            public readonly ushort AF;

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public DaaSample(byte a, bool h, bool n, bool c, ushort af)
            {
                A = a;
                H = h;
                N = n;
                C = c;
                AF = af;
            }
        }
    }
}
