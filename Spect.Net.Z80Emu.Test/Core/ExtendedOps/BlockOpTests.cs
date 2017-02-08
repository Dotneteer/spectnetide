using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.ExtendedOps
{
    [TestClass]
    public class BlockOpTests
    {
        /// <summary>
        /// LDI: 0xED 0xA0
        /// </summary>
        [TestMethod]
        public void LDI_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA0 // LDI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1001].ShouldBe((byte)0xA5);
            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort)0x1001);
            regs.DE.ShouldBe((ushort)0x1002);
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// LDI: 0xED 0xA0
        /// </summary>
        [TestMethod]
        public void LDI_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA0 // LDI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0001;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1001].ShouldBe((byte)0xA5);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x1001);
            regs.DE.ShouldBe((ushort)0x1002);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// CPI: 0xED 0xA1
        /// </summary>
        [TestMethod]
        public void CPI_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA1 // CPI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.A = 0x11;
            m.Memory[regs.HL] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort)0x1001);

            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// CPI: 0xED 0xA1
        /// </summary>
        [TestMethod]
        public void CPI_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA1 // CPI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0001;
            regs.HL = 0x1000;
            regs.A = 0x11;
            m.Memory[regs.HL] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x1001);

            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// CPI: 0xED 0xA1
        /// </summary>
        [TestMethod]
        public void CPI_WorksWithByteFound()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA1 // CPI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.A = 0xA5;
            m.Memory[regs.HL] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort)0x1001);

            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// CPI: 0xED 0xA1
        /// </summary>
        [TestMethod]
        public void CPI_WorksWithByteFoundAndZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA1 // CPI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0001;
            regs.HL = 0x1000;
            regs.A = 0xA5;
            m.Memory[regs.HL] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x1001);

            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// LDD: 0xED 0xA8
        /// </summary>
        [TestMethod]
        public void LDD_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA8 // LDD
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1001].ShouldBe((byte)0xA5);
            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort) 0x0FFF);
            regs.DE.ShouldBe((ushort)0x1000);
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// LDD: 0xED 0xA8
        /// </summary>
        [TestMethod]
        public void LDD_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA8 // LDI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0001;
            regs.HL = 0x1000;
            regs.DE = 0x1001;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.DE] = 0x11;

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1001].ShouldBe((byte)0xA5);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x0FFF);
            regs.DE.ShouldBe((ushort)0x1000);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// LDIR: 0xED 0xB0
        /// </summary>
        [TestMethod]
        public void LDIR_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xB0 // LDIR
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0003;
            regs.HL = 0x1001;
            regs.DE = 0x1000;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.HL+1] = 0xA6;
            m.Memory[regs.HL+2] = 0xA7;

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1000].ShouldBe((byte)0xA5);
            m.Memory[0x1001].ShouldBe((byte)0xA6);
            m.Memory[0x1002].ShouldBe((byte)0xA7);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x1004);
            regs.DE.ShouldBe((ushort)0x1003);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1000-1002");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(58ul);
        }

        /// <summary>
        /// LDDR: 0xED 0xB8
        /// </summary>
        [TestMethod]
        public void LDDR_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xB8 // LDDR
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0003;
            regs.HL = 0x1002;
            regs.DE = 0x1003;
            m.Memory[regs.HL - 2] = 0xA5;
            m.Memory[regs.HL - 1] = 0xA6;
            m.Memory[regs.HL] = 0xA7;

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1001].ShouldBe((byte)0xA5);
            m.Memory[0x1002].ShouldBe((byte)0xA6);
            m.Memory[0x1003].ShouldBe((byte)0xA7);
            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x0FFF);
            regs.DE.ShouldBe((ushort)0x1000);
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, DE, HL");
            m.ShouldKeepMemory(except: "1001-1003");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(58ul);
        }

        /// <summary>
        /// CPD: 0xED 0xA9
        /// </summary>
        [TestMethod]
        public void CPD_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA9 // CPD
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.A = 0x11;
            m.Memory[regs.HL] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// CPD: 0xED 0xA9
        /// </summary>
        [TestMethod]
        public void CPD_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA9 // CPD
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0001;
            regs.HL = 0x1000;
            regs.A = 0x11;
            m.Memory[regs.HL] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// CPD: 0xED 0xA9
        /// </summary>
        [TestMethod]
        public void CPD_WorksWithByteFound()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA9 // CPD
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0010;
            regs.HL = 0x1000;
            regs.A = 0xA5;
            m.Memory[regs.HL] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x000F);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// CPD: 0xED 0xA9
        /// </summary>
        [TestMethod]
        public void CPD_WorksWithByteFoundAndZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA9 // CPD
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0001;
            regs.HL = 0x1000;
            regs.A = 0xA5;
            m.Memory[regs.HL] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(16ul);
        }
    }
}
