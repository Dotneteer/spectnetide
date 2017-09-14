using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;
using Z80TestMachine = Spect.Net.SpectrumEmu.Test.Helpers.Z80TestMachine;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.IndexedBitOps
{
    [TestClass]
    public class IyBitOpTests0X00
    {
        /// <summary>
        /// RLC (IY+D),B: 0XFD 0xCB 0x00
        /// </summary>
        [TestMethod]
        public void XRLC_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x00 // RLC (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.B.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D),B: 0XFD 0xCB 0x00
        /// </summary>
        [TestMethod]
        public void XRLC_B_WorksWithNegativeOffset()
        {
            // --- Arrange
            const byte OFFS = 0xFE;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x00 // RLC (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY - 256 + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY - 256 + OFFS]);
            regs.B.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory(except: "0FFE");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D),B: 0XFD 0xCB 0x00
        /// </summary>
        [TestMethod]
        public void XRLC_B_SetsCarry()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x00 // RLC (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x84;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.B.ShouldBe((byte)0x09);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D),B: 0XFD 0xCB 0x00
        /// </summary>
        [TestMethod]
        public void XRLC_B_SetsZeroFlag()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x00 // RLC (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x00;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.B.ShouldBe((byte)0x00);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
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
        /// RLC (IY+D),B: 0XFD 0xCB 0x00
        /// </summary>
        [TestMethod]
        public void XRLC_B_SetsSignFlag()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x00 // RLC (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0xC0;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.B.ShouldBe((byte)0x81);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D),C: 0XFD 0xCB 0x01
        /// </summary>
        [TestMethod]
        public void XRLC_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x01 // RLC (IY+32H),C
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.C.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D),D: 0XFD 0xCB 0x02
        /// </summary>
        [TestMethod]
        public void XRLC_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x02 // RLC (IY+32H),D
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.D.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D),E: 0XFD 0xCB 0x03
        /// </summary>
        [TestMethod]
        public void XRLC_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x03 // RLC (IY+32H),E
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.E.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D),H: 0XFD 0xCB 0x04
        /// </summary>
        [TestMethod]
        public void XRLC_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x04 // RLC (IY+32H),H
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.H.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, H");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D),L: 0XFD 0xCB 0x05
        /// </summary>
        [TestMethod]
        public void XRLC_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x05 // RLC (IY+32H),L
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.L.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, L");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D): 0XFD 0xCB 0x06
        /// </summary>
        [TestMethod]
        public void XRLC_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x06 // RLC (IY+32H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RLC (IY+D),A: 0XFD 0xCB 0x07
        /// </summary>
        [TestMethod]
        public void XRLC_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x07 // RLC (IY+32H),A
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.A.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, A");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RRC (IY+D),B: 0XFD 0xCB 0x08
        /// </summary>
        [TestMethod]
        public void XRRC_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x08 // RRC (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.B.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RRC (IY+D),B: 0XFD 0xCB 0x08
        /// </summary>
        [TestMethod]
        public void XRRC_B_WorksWithNegativeOffset()
        {
            // --- Arrange
            const byte OFFS = 0xFE;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x08 // RRC (IY+32H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY - 256 + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe(m.Memory[regs.IY - 256 + OFFS]);
            regs.B.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory(except: "0FFE");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RRC (IY+D),C: 0XFD 0xCB 0x09
        /// </summary>
        [TestMethod]
        public void XRRC_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x09 // RRC (IY+32H),C
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.C.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RRC (IY+D),D: 0XFD 0xCB 0x0A
        /// </summary>
        [TestMethod]
        public void XRRC_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x0A // RRC (IY+32H),D
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.D.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RRC (IY+D),E: 0XFD 0xCB 0x0B
        /// </summary>
        [TestMethod]
        public void XRRC_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x0B // RRC (IY+32H),E
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.E.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RRC (IY+D),H: 0XFD 0xCB 0x0C
        /// </summary>
        [TestMethod]
        public void XRRC_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x0C // RRC (IY+32H),H
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.H.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, H");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RRC (IY+D),L: 0XFD 0xCB 0x0D
        /// </summary>
        [TestMethod]
        public void XRRC_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x0D // RRC (IY+32H),L
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.L.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, L");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RRC (IY+D): 0XFD 0xCB 0x0E
        /// </summary>
        [TestMethod]
        public void XRRC_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x0E // RLC (IY+32H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// RRC (IY+D),A: 0XFD 0xCB 0x0F
        /// </summary>
        [TestMethod]
        public void XRRC_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x32;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0XFD, 0xCB, OFFS, 0x0F // RRC (IY+32H),A
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe(m.Memory[regs.IY + OFFS]);
            regs.A.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, A");
            m.ShouldKeepMemory(except: "1032");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(23L);
        }
    }
}