using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
using Z80TestMachine = Spect.Net.Z80Emu.Test.Helpers.Z80TestMachine;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.IndexedBitOps
{
    [TestClass]
    public class IxBitOpTests
    {
        /// <summary>
        /// BIT N,(IDX+D): 0xDD 0xCB 0x40-0x4F
        /// </summary>
        [TestMethod]
        public void BIT_N_B_WorksWithBitReset()
        {
            const byte OFFS = 0x54;
            for (var n = 0; n < 8; n++)
            {
                for (var repeat = 0; repeat < 8; repeat++)
                {
                    // --- Arrange
                    var m = new Z80TestMachine(RunMode.OneInstruction);
                    var opcn = (byte)(0x40 | (n << 3) | repeat);

                    m.InitCode(new byte[]
                    {
                        0xDD, 0xCB, OFFS, opcn // BIT N,(IX+54H)
                    });

                    var regs = m.Cpu.Registers;
                    regs.IX = 0x1000;
                    m.Memory[regs.IX + OFFS] = (byte)~(0x01 << n);

                    // --- Act
                    m.Run();

                    // --- Assert
                    regs.SFlag.ShouldBeFalse();
                    regs.ZFlag.ShouldBeTrue();
                    regs.CFlag.ShouldBeFalse();
                    regs.PFlag.ShouldBeTrue();

                    regs.HFlag.ShouldBeTrue();
                    regs.NFlag.ShouldBeFalse();
                    m.ShouldKeepRegisters(except: "F");
                    m.ShouldKeepMemory();

                    regs.PC.ShouldBe((ushort)0x0004);
                    m.Cpu.Tacts.ShouldBe(20ul);
                }
            }
        }

        /// <summary>
        /// BIT N,(IDX+D): 0xDD 0xCB 0x40-0x4F
        /// </summary>
        [TestMethod]
        public void BIT_N_B_WorksWithBitSet()
        {
            const byte OFFS = 0x54;
            for (var n = 0; n < 8; n++)
            {
                for (var repeat = 0; repeat < 8; repeat++)
                {
                    // --- Arrange
                    var m = new Z80TestMachine(RunMode.OneInstruction);
                    var opcn = (byte)(0x40 | (n << 3) | repeat);

                    m.InitCode(new byte[]
                    {
                        0xDD, 0xCB, OFFS, opcn // BIT N,(IX+54H)
                    });

                    var regs = m.Cpu.Registers;
                    regs.IX = 0x1000;
                    m.Memory[regs.IX + OFFS] = (byte)(0x01 << n);

                    // --- Act
                    m.Run();

                    // --- Assert
                    regs.SFlag.ShouldBe(n == 0x07);
                    regs.ZFlag.ShouldBeFalse();
                    regs.CFlag.ShouldBeFalse();
                    regs.PFlag.ShouldBeFalse();

                    regs.HFlag.ShouldBeTrue();
                    regs.NFlag.ShouldBeFalse();
                    m.ShouldKeepRegisters(except: "F");
                    m.ShouldKeepMemory();

                    regs.PC.ShouldBe((ushort)0x0004);
                    m.Cpu.Tacts.ShouldBe(20ul);
                }
            }
        }

        /// <summary>
        /// RES N,(IDX+D): 0xDD 0xCB 0x80-0x8F
        /// </summary>
        [TestMethod]
        public void RES_N_B_WorksWithBitAlreadyReset()
        {
            const byte OFFS = 0x54;
            for (var n = 0; n < 8; n++)
            {
                for (var repeat = 0; repeat < 8; repeat++)
                {
                    // --- Arrange
                    var m = new Z80TestMachine(RunMode.OneInstruction);
                    var opcn = (byte)(0x80 | (n << 3) | repeat);

                    m.InitCode(new byte[]
                    {
                        0xDD, 0xCB, OFFS, opcn // RES N,(IX+54H)
                    });

                    var regs = m.Cpu.Registers;
                    regs.IX = 0x1000;
                    m.Memory[regs.IX + OFFS] = 0xFF;

                    // --- Act
                    m.Run();

                    // --- Assert
                    m.Memory[regs.IX + OFFS].ShouldBe((byte)(0xFF & ~(1 << n)));
                    m.ShouldKeepMemory(except: "1054");

                    regs.PC.ShouldBe((ushort)0x0004);
                    m.Cpu.Tacts.ShouldBe(23ul);
                }
            }
        }

        /// <summary>
        /// RES N,(IDX+D): 0xDD 0xCB 0x80-0x8F
        /// </summary>
        [TestMethod]
        public void RES_N_B_WorksWithBitPartlyReset()
        {
            const byte OFFS = 0x54;
            for (var n = 0; n < 8; n++)
            {
                for (var repeat = 0; repeat < 8; repeat++)
                {
                    // --- Arrange
                    var m = new Z80TestMachine(RunMode.OneInstruction);
                    var opcn = (byte)(0x80 | (n << 3) | repeat);

                    m.InitCode(new byte[]
                    {
                        0xDD, 0xCB, OFFS, opcn // RES N,(IX+54H)
                    });

                    var regs = m.Cpu.Registers;
                    regs.IX = 0x1000;
                    m.Memory[regs.IX + OFFS] = 0xAA;

                    // --- Act
                    m.Run();

                    // --- Assert
                    m.Memory[regs.IX + OFFS].ShouldBe((byte)(0xAA & ~(1 << n)));
                    m.ShouldKeepMemory(except: "1054");

                    regs.PC.ShouldBe((ushort)0x0004);
                    m.Cpu.Tacts.ShouldBe(23ul);
                }
            }
        }

        /// <summary>
        /// SET N,(IDX+D): 0xDD 0xCB 0xC0-0xFF
        /// </summary>
        [TestMethod]
        public void SET_N_B_WorksWithBitReset()
        {
            const byte OFFS = 0x54;
            for (var n = 0; n < 8; n++)
            {
                for (var repeat = 0; repeat < 8; repeat++)
                {
                    // --- Arrange
                    var m = new Z80TestMachine(RunMode.OneInstruction);
                    var opcn = (byte)(0xC0 | (n << 3) | repeat);

                    m.InitCode(new byte[]
                    {
                        0xDD, 0xCB, OFFS, opcn // SET N,(IX+54H)
                    });

                    var regs = m.Cpu.Registers;
                    regs.IX = 0x1000;
                    m.Memory[regs.IX + OFFS] = 0x00;

                    // --- Act
                    m.Run();

                    // --- Assert
                    m.Memory[regs.IX + OFFS].ShouldBe((byte)(1 << n));
                    m.ShouldKeepMemory(except: "1054");

                    regs.PC.ShouldBe((ushort)0x0004);
                    m.Cpu.Tacts.ShouldBe(23ul);
                }
            }
        }

        /// <summary>
        /// SET N,(IDX+D): 0xDD 0xCB 0xC0-0xFF
        /// </summary>
        [TestMethod]
        public void SET_N_B_WorksWithBitPartlyReset()
        {
            const byte OFFS = 0x54;
            for (var n = 0; n < 8; n++)
            {
                for (var repeat = 0; repeat < 8; repeat++)
                {
                    // --- Arrange
                    var m = new Z80TestMachine(RunMode.OneInstruction);
                    var opcn = (byte)(0xC0 | (n << 3) | repeat);

                    m.InitCode(new byte[]
                    {
                        0xDD, 0xCB, OFFS, opcn // SET N,(IX+54H)
                    });

                    var regs = m.Cpu.Registers;
                    regs.IX = 0x1000;
                    m.Memory[regs.IX + OFFS] = 0x55;

                    // --- Act
                    m.Run();

                    // --- Assert
                    m.Memory[regs.IX + OFFS].ShouldBe((byte)(0x55 | (1 << n)));
                    m.ShouldKeepMemory(except: "1054");

                    regs.PC.ShouldBe((ushort)0x0004);
                    m.Cpu.Tacts.ShouldBe(23ul);
                }
            }
        }
    }
}
