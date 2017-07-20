using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.BitOps
{
    [TestClass]
    public class BitOpTests0X00
    {
        /// <summary>
        /// RLC B: 0xCB 0x00
        /// </summary>
        [TestMethod]
        public void RLC_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x00 // RLC B
            });
            var regs = m.Cpu.Registers;
            regs.B = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RLC B: 0xCB 0x00
        /// </summary>
        [TestMethod]
        public void RLC_B_SetsCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x00 // RLC B
            });
            var regs = m.Cpu.Registers;
            regs.B = 0x84;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0x09);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RLC B: 0xCB 0x00
        /// </summary>
        [TestMethod]
        public void RLC_B_SetsZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x00 // RLC B
            });
            var regs = m.Cpu.Registers;
            regs.B = 0x00;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0x00);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RLC B: 0xCB 0x00
        /// </summary>
        [TestMethod]
        public void RLC_B_SetsSignFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x00 // RLC B
            });
            var regs = m.Cpu.Registers;
            regs.B = 0xC0;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0x81);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RLC C: 0xCB 0x01
        /// </summary>
        [TestMethod]
        public void RLC_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x01 // RLC C
            });
            var regs = m.Cpu.Registers;
            regs.C = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RLC D: 0xCB 0x02
        /// </summary>
        [TestMethod]
        public void RLC_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x02 // RLC D
            });
            var regs = m.Cpu.Registers;
            regs.D = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RLC E: 0xCB 0x03
        /// </summary>
        [TestMethod]
        public void RLC_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x03 // RLC E
            });
            var regs = m.Cpu.Registers;
            regs.E = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RLC H: 0xCB 0x04
        /// </summary>
        [TestMethod]
        public void RLC_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x04 // RLC H
            });
            var regs = m.Cpu.Registers;
            regs.H = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RLC L: 0xCB 0x05
        /// </summary>
        [TestMethod]
        public void RLC_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x05 // RLC L
            });
            var regs = m.Cpu.Registers;
            regs.L = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RLC (HL): 0xCB 0x06
        /// </summary>
        [TestMethod]
        public void RLC_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x06 // RLC (HL)
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x1000;
            m.Memory[regs.HL] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.HL].ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1000");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// RLC A: 0xCB 0x07
        /// </summary>
        [TestMethod]
        public void RLC_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x07 // RLC A
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe((byte)0x10);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC B: 0xCB 0x08
        /// </summary>
        [TestMethod]
        public void RRC_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x08 // RRC B
            });
            var regs = m.Cpu.Registers;
            regs.B = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC B: 0xCB 0x08
        /// </summary>
        [TestMethod]
        public void RRC_B_SetsCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x08 // RRC B
            });
            var regs = m.Cpu.Registers;
            regs.B = 0x85;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0xC2);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC B: 0xCB 0x08
        /// </summary>
        [TestMethod]
        public void RRC_B_SetsZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x00 // RLC B
            });
            var regs = m.Cpu.Registers;
            regs.B = 0x00;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0x00);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC B: 0xCB 0x08
        /// </summary>
        [TestMethod]
        public void RRC_B_SetsSignFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x08 // RRC B
            });
            var regs = m.Cpu.Registers;
            regs.B = 0x41;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0xA0);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC C: 0xCB 0x09
        /// </summary>
        [TestMethod]
        public void RRC_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x09 // RRC C
            });
            var regs = m.Cpu.Registers;
            regs.C = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC D: 0xCB 0x0A
        /// </summary>
        [TestMethod]
        public void RRC_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x0A // RRC D
            });
            var regs = m.Cpu.Registers;
            regs.D = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC E: 0xCB 0x0B
        /// </summary>
        [TestMethod]
        public void RRC_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x0B // RRC E
            });
            var regs = m.Cpu.Registers;
            regs.E = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC H: 0xCB 0x0C
        /// </summary>
        [TestMethod]
        public void RRC_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x0C // RRC H
            });
            var regs = m.Cpu.Registers;
            regs.H = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC L: 0xCB 0x0D
        /// </summary>
        [TestMethod]
        public void RRC_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x0D // RRC L
            });
            var regs = m.Cpu.Registers;
            regs.L = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// RRC (HL): 0xCB 0x0E
        /// </summary>
        [TestMethod]
        public void RRC_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x0E // RRC (HL)
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x1000;
            m.Memory[regs.HL] = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.HL].ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1000");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// RRC A: 0xCB 0x0F
        /// </summary>
        [TestMethod]
        public void RRC_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xCB, 0x0F // RRC A
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x08;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe((byte)0x04);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

    }
}
