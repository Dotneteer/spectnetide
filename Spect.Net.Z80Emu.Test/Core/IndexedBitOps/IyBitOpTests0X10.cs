using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Core;
using Spect.Net.Z80Emu.Test.Helpers;

namespace Spect.Net.Z80Emu.Test.Core.IndexedBitOps
{
    [TestClass]
    public class IyBitOpTests0X10
    {
        /// <summary>
        /// RL (IY+D),B: 0xFD 0xCB 0x10
        /// </summary>
        [TestMethod]
        public void XRL_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x10 // RL (IY+32H),B
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
        /// RL (IY+D),C: 0xFD 0xCB 0x11
        /// </summary>
        [TestMethod]
        public void XRL_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x11 // RL (IY+32H),C
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
        /// RL (IY+D),D: 0xFD 0xCB 0x12
        /// </summary>
        [TestMethod]
        public void XRL_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x12 // RL (IY+32H),D
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
        /// RL (IY+D),E: 0xFD 0xCB 0x13
        /// </summary>
        [TestMethod]
        public void XRL_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x13 // RL (IY+32H),E
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
        /// RL (IY+D),H: 0xFD 0xCB 0x14
        /// </summary>
        [TestMethod]
        public void XRL_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x14 // RL (IY+32H),H
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
        /// RL (IY+D),L: 0xFD 0xCB 0x15
        /// </summary>
        [TestMethod]
        public void XRL_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x15 // RL (IY+32H),L
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
        /// RL (IY+D): 0xFD 0xCB 0x16
        /// </summary>
        [TestMethod]
        public void XRL_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x16 // RL (IY+32H)
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
        /// RL (IY+D),A: 0xFD 0xCB 0x17
        /// </summary>
        [TestMethod]
        public void XRL_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x17 // RL (IY+32H),A
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
        /// RR (IY+D),B: 0xFD 0xCB 0x18
        /// </summary>
        [TestMethod]
        public void XRR_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x18 // RR (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.B.ShouldBe((byte)0x84);

            regs.SFlag.ShouldBeTrue();
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
        /// RR (IY+D),C: 0xFD 0xCB 0x19
        /// </summary>
        [TestMethod]
        public void XRR_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x19 // RR (IY+32H),C
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.C.ShouldBe((byte)0x84);

            regs.SFlag.ShouldBeTrue();
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
        /// RR (IY+D),D: 0xFD 0xCB 0x1A
        /// </summary>
        [TestMethod]
        public void XRR_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x1A // RR (IY+32H),D
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.D.ShouldBe((byte)0x84);

            regs.SFlag.ShouldBeTrue();
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
        /// RR (IY+D),E: 0xFD 0xCB 0x1B
        /// </summary>
        [TestMethod]
        public void XRR_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x1B // RR (IY+32H),E
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.E.ShouldBe((byte)0x84);

            regs.SFlag.ShouldBeTrue();
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
        /// RR (IY+D),H: 0xFD 0xCB 0x1C
        /// </summary>
        [TestMethod]
        public void XRR_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x1C // RR (IY+32H),H
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.H.ShouldBe((byte)0x84);

            regs.SFlag.ShouldBeTrue();
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
        /// RR (IY+D),L: 0xFD 0xCB 0x1D
        /// </summary>
        [TestMethod]
        public void XRR_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x1D // RR (IY+32H),L
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.L.ShouldBe((byte)0x84);

            regs.SFlag.ShouldBeTrue();
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
        /// RR (IY+D): 0xFD 0xCB 0x1E
        /// </summary>
        [TestMethod]
        public void XRR_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x1E // RR (IY+32H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0x84);

            regs.SFlag.ShouldBeTrue();
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
        /// RR (IY+D),A: 0xFD 0xCB 0x1F
        /// </summary>
        [TestMethod]
        public void XRR_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xCB, OFFS, 0x1F // RR (IY+32H),A
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.A.ShouldBe((byte)0x84);

            regs.SFlag.ShouldBeTrue();
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
    }
}