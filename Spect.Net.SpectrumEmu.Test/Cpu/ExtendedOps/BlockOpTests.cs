using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;
using Z80TestMachine = Spect.Net.SpectrumEmu.Test.Helpers.Z80TestMachine;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.ExtendedOps
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// INI: 0xED 0xA2
        /// </summary>
        [TestMethod]
        public void INI_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA2 // INI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x10CC;
            regs.HL = 0x1000;
            m.IoInputSequence.Add(0x69);

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1000].ShouldBe((byte)0x69);
            regs.B.ShouldBe((byte)0x0F);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x1001);

            regs.ZFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory(except: "1000");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// INI: 0xED 0xA2
        /// </summary>
        [TestMethod]
        public void INI_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA2 // INI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x01CC;
            regs.HL = 0x1000;
            m.IoInputSequence.Add(0x69);

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1000].ShouldBe((byte)0x69);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x1001);

            regs.ZFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory(except: "1000");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// OUTI: 0xED 0xA3
        /// </summary>
        [TestMethod]
        public void OUTI_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA3 // OUTI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x10CC;
            regs.HL = 0x1000;
            m.Memory[regs.HL] = 0x29;

            // --- Act
            m.Run();

            // --- Assert

            regs.B.ShouldBe((byte)0x0F);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x1001);

            regs.ZFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x0FCC);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x29);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// OUTI: 0xED 0xA3
        /// </summary>
        [TestMethod]
        public void OUTI_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xA3 // OUTI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x01CC;
            regs.HL = 0x1000;
            m.Memory[regs.HL] = 0x29;

            // --- Act
            m.Run();

            // --- Assert

            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x1001);

            regs.ZFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory(except: "1000");

            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x00CC);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x29);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// IND: 0xED 0xAA
        /// </summary>
        [TestMethod]
        public void IND_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xAA // IND
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x10CC;
            regs.HL = 0x1000;
            m.IoInputSequence.Add(0x69);

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1000].ShouldBe((byte)0x69);
            regs.B.ShouldBe((byte)0x0F);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory(except: "1000");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// IND: 0xED 0xAA
        /// </summary>
        [TestMethod]
        public void IND_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xAA // IND
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x01CC;
            regs.HL = 0x1000;
            m.IoInputSequence.Add(0x69);

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1000].ShouldBe((byte)0x69);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory(except: "1000");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// OUTD: 0xED 0xAB
        /// </summary>
        [TestMethod]
        public void OUTD_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xAB // OUTD
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x10CC;
            regs.HL = 0x1000;
            m.Memory[regs.HL] = 0x29;

            // --- Act
            m.Run();

            // --- Assert

            regs.B.ShouldBe((byte)0x0F);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory(except: "1000");

            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x0FCC);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x29);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// OUTD: 0xED 0xAB
        /// </summary>
        [TestMethod]
        public void OUTD_WorksWithZeroedCounter()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0xAB // OUTD
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x01CC;
            regs.HL = 0x1000;
            m.Memory[regs.HL] = 0x29;

            // --- Act
            m.Run();

            // --- Assert

            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory(except: "1000");

            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x00CC);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x29);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(16L);
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
            m.Cpu.Tacts.ShouldBe(58L);
        }

        /// <summary>
        /// CPIR: 0xED 0xB1
        /// </summary>
        [TestMethod]
        public void CPIR_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xB1 // CPIR
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0003;
            regs.HL = 0x1000;
            regs.A = 0x11;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.HL + 1] = 0xA6;
            m.Memory[regs.HL + 2] = 0xA7;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x0000);
            regs.HL.ShouldBe((ushort)0x1003);

            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(58L);
        }

        /// <summary>
        /// CPIR: 0xED 0xB1
        /// </summary>
        [TestMethod]
        public void CPIR_WorksWithByteFound()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xB1 // CPIR
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0003;
            regs.HL = 0x1000;
            regs.A = 0xA6;
            m.Memory[regs.HL] = 0xA5;
            m.Memory[regs.HL + 1] = 0xA6;
            m.Memory[regs.HL + 2] = 0xA7;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x0001);
            regs.HL.ShouldBe((ushort)0x1002);

            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(37L);
        }

        /// <summary>
        /// INIR: 0xED 0xB2
        /// </summary>
        [TestMethod]
        public void INIR_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xB2 // INIR
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x03CC;
            regs.HL = 0x1000;
            m.IoInputSequence.Add(0x69);
            m.IoInputSequence.Add(0x6A);
            m.IoInputSequence.Add(0x6B);

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1000].ShouldBe((byte)0x69);
            m.Memory[0x1001].ShouldBe((byte)0x6A);
            m.Memory[0x1002].ShouldBe((byte)0x6B);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x1003);

            regs.ZFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory(except: "1000-1002");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(58L);
        }

        /// <summary>
        /// OTIR: 0xED 0xB3
        /// </summary>
        [TestMethod]
        public void OTIR_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xB3 // OUTI
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x3CC;
            regs.HL = 0x1000;
            m.Memory[regs.HL] = 0x29;
            m.Memory[regs.HL + 1] = 0x2A;
            m.Memory[regs.HL + 2] = 0x2B;

            // --- Act
            m.Run();

            // --- Assert

            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x1003);

            regs.ZFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            m.IoAccessLog.Count.ShouldBe(3);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x02CC);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x29);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();
            m.IoAccessLog[1].Address.ShouldBe((ushort)0x01CC);
            m.IoAccessLog[1].Value.ShouldBe((byte)0x2A);
            m.IoAccessLog[1].IsOutput.ShouldBeTrue();
            m.IoAccessLog[2].Address.ShouldBe((ushort)0x00CC);
            m.IoAccessLog[2].Value.ShouldBe((byte)0x2B);
            m.IoAccessLog[2].IsOutput.ShouldBeTrue();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(58L);
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
            m.Cpu.Tacts.ShouldBe(58L);
        }

        /// <summary>
        /// CPDR: 0xED 0xB9
        /// </summary>
        [TestMethod]
        public void CPDR_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xB9 // CPDR
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0003;
            regs.HL = 0x1002;
            regs.A = 0x11;
            m.Memory[regs.HL - 2] = 0xA5;
            m.Memory[regs.HL - 1] = 0xA6;
            m.Memory[regs.HL] = 0xA7;

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
            m.Cpu.Tacts.ShouldBe(58L);
        }

        /// <summary>
        /// CPDR: 0xED 0xB9
        /// </summary>
        [TestMethod]
        public void CPDR_WorksWithByteFound()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xB9 // CPDR
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x0003;
            regs.HL = 0x1002;
            regs.A = 0xA6;
            m.Memory[regs.HL - 2] = 0xA5;
            m.Memory[regs.HL - 1] = 0xA6;
            m.Memory[regs.HL] = 0xA7;

            // --- Act
            m.Run();

            // --- Assert

            regs.BC.ShouldBe((ushort)0x0001);
            regs.HL.ShouldBe((ushort)0x1000);

            regs.ZFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(37L);
        }

        /// <summary>
        /// INDR: 0xED 0xBA
        /// </summary>
        [TestMethod]
        public void INDR_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xBA // INDR
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x03CC;
            regs.HL = 0x1002;
            m.IoInputSequence.Add(0x69);
            m.IoInputSequence.Add(0x6A);
            m.IoInputSequence.Add(0x6B);

            // --- Act
            m.Run();

            // --- Assert

            m.Memory[0x1000].ShouldBe((byte)0x6B);
            m.Memory[0x1001].ShouldBe((byte)0x6A);
            m.Memory[0x1002].ShouldBe((byte)0x69);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory(except: "1000-1002");

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(58L);
        }

        /// <summary>
        /// OTDR: 0xED 0xBB
        /// </summary>
        [TestMethod]
        public void OTDR_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xED, 0xBB // OTDR
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x3CC;
            regs.HL = 0x1002;
            m.Memory[regs.HL - 2] = 0x29;
            m.Memory[regs.HL - 1] = 0x2A;
            m.Memory[regs.HL] = 0x2B;

            // --- Act
            m.Run();

            // --- Assert

            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0xCC);
            regs.HL.ShouldBe((ushort)0x0FFF);

            regs.ZFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F, BC, HL");
            m.ShouldKeepMemory();

            m.IoAccessLog.Count.ShouldBe(3);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x02CC);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x2B);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();
            m.IoAccessLog[1].Address.ShouldBe((ushort)0x01CC);
            m.IoAccessLog[1].Value.ShouldBe((byte)0x2A);
            m.IoAccessLog[1].IsOutput.ShouldBeTrue();
            m.IoAccessLog[2].Address.ShouldBe((ushort)0x00CC);
            m.IoAccessLog[2].Value.ShouldBe((byte)0x29);
            m.IoAccessLog[2].IsOutput.ShouldBeTrue();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(58L);
        }
    }
}
