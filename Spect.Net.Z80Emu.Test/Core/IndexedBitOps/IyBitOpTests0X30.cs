using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Core;
using Spect.Net.Z80Emu.Test.Helpers;
using Spect.Net.Z80TestHelpers;

namespace Spect.Net.Z80Emu.Test.Core.IndexedBitOps
{
    [TestClass]
    public class IyBitOpTests0X30
    {
        /// <summary>
        /// SLL (IY+D),B: 0xFD 0xCB 0x30
        /// </summary>
        [TestMethod]
        public void XSLL_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x30 // SLL (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.B.ShouldBe((byte)0x11);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SLL (IY+D),C: 0xFD 0xCB 0x31
        /// </summary>
        [TestMethod]
        public void XSLL_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x31 // SLL (IY+32H),C
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.C.ShouldBe((byte)0x11);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SLL (IY+D),D: 0xFD 0xCB 0x32
        /// </summary>
        [TestMethod]
        public void XSLL_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x32 // SLL (IY+32H),D
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.D.ShouldBe((byte)0x11);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SLL (IY+D),E: 0xFD 0xCB 0x33
        /// </summary>
        [TestMethod]
        public void XSLL_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x33 // SLL (IY+32H),E
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.E.ShouldBe((byte)0x11);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SLL (IY+D),H: 0xFD 0xCB 0x34
        /// </summary>
        [TestMethod]
        public void XSLL_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x34 // SLL (IY+32H),H
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.H.ShouldBe((byte)0x11);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, H");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SLL (IY+D),L: 0xFD 0xCB 0x35
        /// </summary>
        [TestMethod]
        public void XSLL_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x35 // SLL (IY+32H),L
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.L.ShouldBe((byte)0x11);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, L");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SLL (IY+D): 0xFD 0xCB 0x36
        /// </summary>
        [TestMethod]
        public void XSLL_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x36 // SLL (IY+32H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0x11);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SLL (IY+D),A: 0xFD 0xCB 0x37
        /// </summary>
        [TestMethod]
        public void XSLL_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x37 // SLL (IY+32H),A
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.A.ShouldBe((byte)0x11);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, A");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SRL (IY+D),B: 0xFD 0xCB 0x38
        /// </summary>
        [TestMethod]
        public void XSRL_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x38 // SRL (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x10;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.B.ShouldBe((byte)0x08);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SRL (IY+D),C: 0xFD 0xCB 0x39
        /// </summary>
        [TestMethod]
        public void XSRL_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x39 // SRL (IY+32H),C
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x10;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.C.ShouldBe((byte)0x08);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SRL (IY+D),D: 0xFD 0xCB 0x3A
        /// </summary>
        [TestMethod]
        public void XSRL_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x3A // SRL (IY+32H),D
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x10;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.D.ShouldBe((byte)0x08);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SRL (IY+D),E: 0xFD 0xCB 0x3B
        /// </summary>
        [TestMethod]
        public void XSRL_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x3B // SRL (IY+32H),E
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x10;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.E.ShouldBe((byte)0x08);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SRL (IY+D),H: 0xFD 0xCB 0x3C
        /// </summary>
        [TestMethod]
        public void XSRL_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x3C // SRL (IY+32H),H
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x10;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.H.ShouldBe((byte)0x08);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, H");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SRL (IY+D),L: 0xFD 0xCB 0x3D
        /// </summary>
        [TestMethod]
        public void XSRL_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x3D // SRL (IY+32H),L
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x10;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.L.ShouldBe((byte)0x08);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, L");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SRL (IY+D): 0xFD 0xCB 0x3E
        /// </summary>
        [TestMethod]
        public void XSRL_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x3E // SRL (IY+32H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x10;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0x08);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, A");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// SRL (IY+D),A: 0xFD 0xCB 0x3F
        /// </summary>
        [TestMethod]
        public void XSRL_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x3F // SRL (IY+32H),A
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x10;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.A.ShouldBe((byte)0x08);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, A");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(23ul);
        }
    }
}