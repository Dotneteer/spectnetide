using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core
{
    [TestClass]
    public class StandardOpTests0X60
    {
        /// <summary>
        /// LD H,B: 0x60
        /// </summary>
        [TestMethod]
        public void LD_H_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xB9, // LD B,B9H
                0x60        // LD H,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.H.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "H, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD H,C: 0x61
        /// </summary>
        [TestMethod]
        public void LD_H_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0xB9, // LD C,B9H
                0x61        // LD H,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.H.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "H, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD H,E: 0x63
        /// </summary>
        [TestMethod]
        public void LD_H_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0xB9, // LD E,B9H
                0x63        // LD H,E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.H.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "H, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD H,L: 0x65
        /// </summary>
        [TestMethod]
        public void LD_H_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x2E, 0xB9, // LD L,B9H
                0x65        // LD H,L
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.H.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "H, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD H,(HL): 0x66
        /// </summary>
        [TestMethod]
        public void LD_D_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0x66              // LD H,(HL)
            });
            m.Memory[0x1000] = 0xB9;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.H.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(17ul);
        }

        /// <summary>
        /// LD H,A: 0x67
        /// </summary>
        [TestMethod]
        public void LD_H_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xB9, // LD A,B9H
                0x67        // LD H,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.H.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "H, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD L,B: 0x68
        /// </summary>
        [TestMethod]
        public void LD_L_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xB9, // LD B,B9H
                0x68        // LD L,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.L.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "L, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD L,C: 0x69
        /// </summary>
        [TestMethod]
        public void LD_E_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x0E, 0xB9, // LD C,B9H
                0x69        // LD L,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.L.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "L, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD L,D: 0x6A
        /// </summary>
        [TestMethod]
        public void LD_L_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x16, 0xB9, // LD D,B9H
                0x6A        // LD L,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.L.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "L, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD L,E: 0x6B
        /// </summary>
        [TestMethod]
        public void LD_L_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x1E, 0xB9, // LD E,B9H
                0x6B        // LD L,E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.L.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "L, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD L,H: 0x6C
        /// </summary>
        [TestMethod]
        public void LD_L_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x26, 0xB9, // LD H,B9H
                0x6C        // LD L,H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.L.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "L, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD L,(HL): 0x6E
        /// </summary>
        [TestMethod]
        public void LD_L_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0x6E              // LD L,(HL)
            });
            m.Memory[0x1000] = 0xB9;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.L.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(17ul);
        }

        /// <summary>
        /// LD L,A: 0x6F
        /// </summary>
        [TestMethod]
        public void LD_L_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xB9, // LD A,B9H
                0x6F        // LD L,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.L.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "L, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }
    }
}