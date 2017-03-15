using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
using Z80TestMachine = Spect.Net.Z80Emu.Test.Helpers.Z80TestMachine;

namespace Spect.Net.Z80Emu.Test.Core.StandardOps
{
    [TestClass]
    public class StandardOpTests0xA0
    {
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
        }

        /// <summary>
        /// AND H: 0xA4
        /// </summary>
        [TestMethod]
        public void AND_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x26, 0x23, // LD H,23H
                0xA4        // AND H
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
            m.ShouldKeepRegisters(except: "AF, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
        }

        /// <summary>
        /// AND L: 0xA5
        /// </summary>
        [TestMethod]
        public void AND_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x2E, 0x23, // LD L,23H
                0xA5        // AND L
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
            m.ShouldKeepRegisters(except: "AF, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
        }

        /// <summary>
        /// AND (HL): 0xA6
        /// </summary>
        [TestMethod]
        public void AND_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,        // LD A,12H
                0x21, 0x00, 0x10,  // LD HL,1000H
                0xA6               // AND (HL)
            });
            m.Memory[0x1000] = 0x23;

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
            m.ShouldKeepRegisters(except: "AF, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(24ul);
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

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
        }

        /// <summary>
        /// XOR B: 0xA0
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
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

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
        }

        /// <summary>
        /// XOR H: 0xAC
        /// </summary>
        [TestMethod]
        public void XOR_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x26, 0x23, // LD H,23H
                0xAC        // XOR H
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
            m.ShouldKeepRegisters(except: "AF, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
        }

        /// <summary>
        /// XOR L: 0xAD
        /// </summary>
        [TestMethod]
        public void XOR_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0x2E, 0x23, // LD L,23H
                0xAD        // XOR L
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
            m.ShouldKeepRegisters(except: "AF, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18ul);
        }

        /// <summary>
        /// XOR (HL): 0xAE
        /// </summary>
        [TestMethod]
        public void XOR_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,       // LD A,12H
                0x21, 0x00, 0x10, // LD HL,1000H
                0xAE              // XOR (HL)
            });
            m.Memory[0x1000] = 0x23;

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
            m.ShouldKeepRegisters(except: "AF, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(24ul);
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

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11ul);
        }
    }
}
