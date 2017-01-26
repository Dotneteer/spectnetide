using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Core;
// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Emu.Test.Core
{
    [TestClass]
    public class Z80ExecutionCycleTest
    {
        [TestMethod]
        public void FullResetWhenCreatingZ80()
        {
            // --- Act
            var z80 = new Z80();

            // --- Assert
            z80.Registers.AF.ShouldBe((ushort)0);
            z80.Registers.BC.ShouldBe((ushort)0);
            z80.Registers.DE.ShouldBe((ushort)0);
            z80.Registers.HL.ShouldBe((ushort)0);
            z80.Registers._AF_.ShouldBe((ushort)0);
            z80.Registers._BC_.ShouldBe((ushort)0);
            z80.Registers._DE_.ShouldBe((ushort)0);
            z80.Registers._HL_.ShouldBe((ushort)0);
            z80.Registers.PC.ShouldBe((ushort)0);
            z80.Registers.SP.ShouldBe((ushort)0);
            z80.Registers.IX.ShouldBe((ushort)0);
            z80.Registers.IY.ShouldBe((ushort)0);
            z80.Registers.IR.ShouldBe((ushort)0);
            z80.Registers.MW.ShouldBe((ushort)0);

            z80.RST.ShouldBeFalse();
            z80.INT.ShouldBeFalse();
            z80.INT_BLOCKED.ShouldBeFalse();
            z80.NMI.ShouldBeFalse();
            z80.IM.ShouldBe((byte)0);
            z80.PrefixMode.ShouldBe(Z80.OpPrefixMode.None);
            z80.IndexMode.ShouldBe(Z80.OpIndexMode.None);
            z80.Ticks.ShouldBe(0ul);
        }

        [TestMethod]
        public void HaltedStateExecutesNops()
        {
            // --- Arrange
            var z80 = new Z80();
            z80.HALTED = true;

            // --- Act
            var ticksBefore = z80.Ticks;
            var regRBefore = z80.Registers.R;

            z80.ExecuteCpuCycle();
            var ticksAfter = z80.Ticks;
            var regRAfter = z80.Registers.R;

            // --- Assert
            ticksBefore.ShouldBe(0ul);
            regRBefore.ShouldBe((byte)0x00);
            ticksAfter.ShouldBe(4ul);
            regRAfter.ShouldBe((byte)0x01);
            z80.HALTED.ShouldBeTrue();
        }

        [TestMethod]
        public void RSTSignalIsProcessed()
        {
            // --- Arrange
            var z80 = new Z80();
            z80.Registers.AF = 0x0001;
            z80.Registers.BC = 0x2345;
            z80.Registers.DE = 0x3456;
            z80.Registers.HL = 0x4567;
            z80.Registers.SP = 0x5678;
            z80.Registers.PC = 0x6789;
            z80.Registers.IR = 0x789A;
            z80.Registers.IX = 0x89AB;
            z80.Registers.IY = 0x9ABC;
            z80.Registers._AF_ = 0x9876;
            z80.Registers._BC_ = 0x8765;
            z80.Registers._DE_ = 0x7654;
            z80.Registers._HL_ = 0x6543;

            z80.INT_BLOCKED = true;
            z80.IFF1 = true;
            z80.IFF2 = true;
            z80.PrefixMode = Z80.OpPrefixMode.Bit;
            z80.IndexMode = Z80.OpIndexMode.IY;
            z80.IM = 2;
            z80.Ticks = 1000;

            // --- Act
            z80.RST = true;
            z80.ExecuteCpuCycle();

            // --- Assert
            z80.Registers.AF.ShouldBe((ushort)0x0001);
            z80.Registers.BC.ShouldBe((ushort)0x2345);
            z80.Registers.DE.ShouldBe((ushort)0x3456);
            z80.Registers.HL.ShouldBe((ushort)0x4567);
            z80.Registers.SP.ShouldBe((ushort)0x5678);
            z80.Registers.PC.ShouldBe((ushort)0);
            z80.Registers.IR.ShouldBe((ushort)0);
            z80.Registers.IX.ShouldBe((ushort)0x89AB);
            z80.Registers.IY.ShouldBe((ushort)0x9ABC);
            z80.Registers._AF_.ShouldBe((ushort)0x9876);
            z80.Registers._BC_.ShouldBe((ushort)0x8765);
            z80.Registers._DE_.ShouldBe((ushort)0x7654);
            z80.Registers._HL_.ShouldBe((ushort)0x6543);

            z80.INT_BLOCKED.ShouldBeFalse();
            z80.IFF1.ShouldBeFalse();
            z80.IFF2.ShouldBeFalse();
            z80.PrefixMode.ShouldBe(Z80.OpPrefixMode.None);
            z80.IndexMode.ShouldBe(Z80.OpIndexMode.None);
            z80.IM.ShouldBe((byte)0);
            z80.Ticks.ShouldBe(1000ul);
            z80.RST.ShouldBeTrue();
        }
    }
}
