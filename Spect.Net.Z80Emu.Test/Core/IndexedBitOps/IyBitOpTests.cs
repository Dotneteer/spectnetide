using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;

namespace Spect.Net.Z80Emu.Test.Core.IndexedBitOps
{
    [TestClass]
    public class IyBitOpTests
    {
        /// <summary>
        /// BIT N,(IYDX+D): 0XFD 0xCB 0x40-0x4F
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
                        0XFD, 0xCB, OFFS, opcn // BIT N,(IY+54H)
                    });

                    var regs = m.Cpu.Registers;
                    regs.IY = 0x1000;
                    m.Memory[regs.IY + OFFS] = (byte)~(0x01 << n);

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
                    m.Cpu.Ticks.ShouldBe(20ul);
                }
            }
        }

        /// <summary>
        /// BIT N,(IYDX+D): 0XFD 0xCB 0x40-0x4F
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
                        0XFD, 0xCB, OFFS, opcn // BIT N,(IY+54H)
                    });

                    var regs = m.Cpu.Registers;
                    regs.IY = 0x1000;
                    m.Memory[regs.IY + OFFS] = (byte)(0x01 << n);

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
                    m.Cpu.Ticks.ShouldBe(20ul);
                }
            }
        }
    }
}