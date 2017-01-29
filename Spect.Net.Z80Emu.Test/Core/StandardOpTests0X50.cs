using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core
{
    [TestClass]
    public class StandardOpTests0X50
    {
        /// <summary>
        /// LD D,B: 0x50
        /// </summary>
        [TestMethod]
        public void LD_B_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xB9, // LD B,B9H
                0x50        // LD D,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x51        // LD D,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x53        // LD D,E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "D, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD D,H: 0x54
        /// </summary>
        [TestMethod]
        public void LD_D_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x26, 0xB9, // LD H,B9H
                0x54        // LD D,H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "D, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD D,L: 0x55
        /// </summary>
        [TestMethod]
        public void LD_D_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x2E, 0xB9, // LD L,B9H
                0x55        // LD D,L
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "D, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD D,(HL): 0x56
        /// </summary>
        [TestMethod]
        public void LD_D_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0x56              // LD D,(HL)
            });
            m.Memory[0x1000] = 0xB9;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "D, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(17ul);
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
                0x57        // LD D,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.D.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "D, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x58        // LD E,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "E, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x59        // LD E,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "E, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x5A        // LD E,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "DE");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD E,H: 0x5C
        /// </summary>
        [TestMethod]
        public void LD_E_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x26, 0xB9, // LD H,B9H
                0x5C        // LD E,H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "E, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD E,L: 0x5D
        /// </summary>
        [TestMethod]
        public void LD_E_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x2E, 0xB9, // LD L,B9H
                0x5D        // LD E,L
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "E, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD E,(HL): 0x5E
        /// </summary>
        [TestMethod]
        public void LD_E_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0x5E              // LD E,(HL)
            });
            m.Memory[0x1000] = 0xB9;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "E, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(17ul);
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
                0x5F        // LD E,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.E.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "E, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }
    }
}