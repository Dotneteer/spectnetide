using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Cpu;
using Spect.Net.SpectrumEmu.Test.Helpers;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.IndexedBitOps
{
    [TestClass]
    public class IxBitOpTests0X10
    {
        /// <summary>
        /// RL (IX+D),B: 0xDD 0xCB 0x10
        /// </summary>
        [TestMethod]
        public void XRL_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x10 // RL (IX+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RL (IX+D),C: 0xDD 0xCB 0x11
        /// </summary>
        [TestMethod]
        public void XRL_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x11 // RL (IX+32H),C
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RL (IX+D),D: 0xDD 0xCB 0x12
        /// </summary>
        [TestMethod]
        public void XRL_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x12 // RL (IX+32H),D
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RL (IX+D),E: 0xDD 0xCB 0x13
        /// </summary>
        [TestMethod]
        public void XRL_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x13 // RL (IX+32H),E
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RL (IX+D),H: 0xDD 0xCB 0x14
        /// </summary>
        [TestMethod]
        public void XRL_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x14 // RL (IX+32H),H
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RL (IX+D),L: 0xDD 0xCB 0x15
        /// </summary>
        [TestMethod]
        public void XRL_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x15 // RL (IX+32H),L
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RL (IX+D): 0xDD 0xCB 0x16
        /// </summary>
        [TestMethod]
        public void XRL_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x16 // RL (IX+32H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0x11);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RL (IX+D),A: 0xDD 0xCB 0x17
        /// </summary>
        [TestMethod]
        public void XRL_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x17 // RL (IX+32H),A
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RR (IX+D),B: 0xDD 0xCB 0x18
        /// </summary>
        [TestMethod]
        public void XRR_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x18 // RR (IX+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RR (IX+D),C: 0xDD 0xCB 0x19
        /// </summary>
        [TestMethod]
        public void XRR_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x19 // RR (IX+32H),C
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RR (IX+D),D: 0xDD 0xCB 0x1A
        /// </summary>
        [TestMethod]
        public void XRR_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x1A // RR (IX+32H),D
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RR (IX+D),E: 0xDD 0xCB 0x1B
        /// </summary>
        [TestMethod]
        public void XRR_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x1B // RR (IX+32H),E
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RR (IX+D),H: 0xDD 0xCB 0x1C
        /// </summary>
        [TestMethod]
        public void XRR_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x1C // RR (IX+32H),H
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RR (IX+D),L: 0xDD 0xCB 0x1D
        /// </summary>
        [TestMethod]
        public void XRR_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x1D // RR (IX+32H),L
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RR (IX+D): 0xDD 0xCB 0x1E
        /// </summary>
        [TestMethod]
        public void XRR_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x1E // RR (IX+32H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0x84);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, A");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RR (IX+D),A: 0xDD 0xCB 0x1F
        /// </summary>
        [TestMethod]
        public void XRR_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xCB, OFFS, 0x1F // RR (IX+32H),A
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.F |= FlagsSetMask.C;
            m.Memory[regs.IX + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe(m.Memory[regs.IX + OFFS]);
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
            m.Cpu.Tacts.ShouldBe(23L);
        }
    }
}
