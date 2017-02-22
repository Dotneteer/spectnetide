using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
using Z80TestMachine = Spect.Net.Z80Emu.Test.Helpers.Z80TestMachine;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.StandardOps
{
    [TestClass]
    public class StandardOpTests0X40
    {
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
                0x41        // LD B,C
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x42        // LD B,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x43        // LD E,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD B,H: 0x44
        /// </summary>
        [TestMethod]
        public void LD_B_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x26, 0xB9, // LD H,B9H
                0x44        // LD B,H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD B,L: 0x45
        /// </summary>
        [TestMethod]
        public void LD_B_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x2E, 0xB9, // LD L,B9H
                0x45        // LD B,L
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD B,(HL): 0x46
        /// </summary>
        [TestMethod]
        public void LD_B_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0x46              // LD B,(HL)
            });
            m.Memory[0x1000] = 0xB9;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(17ul);
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
                0x47        // LD B,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.B.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x48        // LD C,B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "B, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x4A        // LD C,D
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
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
                0x4B        // LD C,E
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD C,H: 0x4C
        /// </summary>
        [TestMethod]
        public void LD_C_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x26, 0xB9, // LD H,B9H
                0x4C        // LD C,H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD C,L: 0x4D
        /// </summary>
        [TestMethod]
        public void LD_C_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x2E, 0xB9, // LD L,B9H
                0x4D        // LD C,L
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// LD C,(HL): 0x4E
        /// </summary>
        [TestMethod]
        public void LD_C_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0x4E              // LD C,(HL)
            });
            m.Memory[0x1000] = 0xB9;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(17ul);
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
                0x4F        // LD C,A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.C.ShouldBe((byte)0xB9);

            m.ShouldKeepRegisters(except: "C, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }
    }
}