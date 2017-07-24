using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

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
                new DaaSample(0xA9, false, false, false, 0x090C),
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
