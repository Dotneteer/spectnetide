using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
using Spect.Net.Z80TestHelpers;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.StandardOps
{
    [TestClass]
    public class StandardOpTests0X10
    {
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
                0x10, 0x02  // DJNZ 02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(15ul);
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
                0x10, 0x02  // DJNZ 02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(20ul);
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
                0x11, 0x26, 0xA9 // LD DE,A926H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.DE.ShouldBe((ushort)0xA926);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(10ul);
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
                0x12              // LD (BC),A
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
            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(24ul);
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
                0x13              // INC DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.DE.ShouldBe((ushort)0xA927);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(16ul);
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
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x16, 0x26 // LD B,26H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "D");
            m.ShouldKeepMemory();

            regs.D.ShouldBe((byte)0x26);
            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(7ul);
        }

        /// <summary>
        /// RLA: 0x17
        /// </summary>
        [TestMethod]
        public void RLCA_WorksAsExpected()
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
            m.Cpu.Ticks.ShouldBe(11ul);
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
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(15ul);
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
                0x18, 0x20  // JR 20H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();
            regs.PC.ShouldBe((ushort)0x0024);
            m.Cpu.Ticks.ShouldBe(19ul);
        }

        /// <summary>
        /// ADD HL,DE: 0x19
        /// </summary>
        [TestMethod]
        public void ADD_HL_BC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x34, 0x12, // LD HL,1234H
                0x11, 0x02, 0x11, // LD DE,1102H
                0x19              // ADD HL,DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F, DE, HL");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.HL.ShouldBe((ushort)0x2336);
            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Ticks.ShouldBe(31ul);
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
                0x11, 0x03, 0x00, // LD DE,0003H
                0x1A              // LD A,(DE)
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "DE, A");
            m.ShouldKeepMemory();

            regs.A.ShouldBe((byte)0x1A);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(17ul);
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
                0x1B              // DEC DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.DE.ShouldBe((ushort)0xA925);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(16ul);
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
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x1E, 0x26 // LD E,26H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "E");
            m.ShouldKeepMemory();

            regs.E.ShouldBe((byte)0x26);
            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(7ul);
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
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(15ul);
        }
    }
}