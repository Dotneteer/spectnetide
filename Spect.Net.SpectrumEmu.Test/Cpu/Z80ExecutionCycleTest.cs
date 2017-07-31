using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Cpu;

// ReSharper disable InconsistentNaming

namespace Spect.Net.SpectrumEmu.Test.Cpu
{
    [TestClass]
    public class Z80ExecutionCycleTest
    {
        [TestMethod]
        public void FullResetWhenCreatingZ80()
        {
            // --- Act
            var z80 = new Z80Cpu(new Z80TestMemoryDevice(), new Z80TestPortDevice());

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

            (z80.StateFlags & Z80StateFlags.Reset).ShouldBe(Z80StateFlags.None);
            (z80.StateFlags & Z80StateFlags.Int).ShouldBe(Z80StateFlags.None);
            z80.IsInterruptBlocked.ShouldBeFalse();
            (z80.StateFlags & Z80StateFlags.Nmi).ShouldBe(Z80StateFlags.None);
            z80.InterruptMode.ShouldBe((byte)0);
            z80.PrefixMode.ShouldBe(Z80Cpu.OpPrefixMode.None);
            z80.IndexMode.ShouldBe(Z80Cpu.OpIndexMode.None);
            z80.Tacts.ShouldBe(0L);
        }

        [TestMethod]
        public void HaltedStateExecutesNops()
        {
            // --- Arrange
            var z80 = new Z80Cpu(new Z80TestMemoryDevice(), new Z80TestPortDevice());
            z80.StateFlags |= Z80StateFlags.Halted;

            // --- Act
            var ticksBefore = z80.Tacts;
            var regRBefore = z80.Registers.R;

            z80.ExecuteCpuCycle();
            var ticksAfter = z80.Tacts;
            var regRAfter = z80.Registers.R;

            // --- Assert
            ticksBefore.ShouldBe(0L);
            regRBefore.ShouldBe((byte)0x00);
            ticksAfter.ShouldBe(4L);
            regRAfter.ShouldBe((byte)0x01);
            (z80.StateFlags & Z80StateFlags.Halted).ShouldBe(Z80StateFlags.Halted);

        }

        [TestMethod]
        public void RSTSignalIsProcessed()
        {
            // --- Arrange
            var z80 = new Z80Cpu(new Z80TestMemoryDevice(), new Z80TestPortDevice());
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

            z80.BlockInterrupt();
            z80.IFF1 = true;
            z80.IFF2 = true;
            z80.PrefixMode = Z80Cpu.OpPrefixMode.Bit;
            z80.IndexMode = Z80Cpu.OpIndexMode.IY;
            z80.SetInterruptMode(2);
            z80.SetTacts(1000);

            // --- Act
            z80.StateFlags = Z80StateFlags.Reset;
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

            z80.IsInterruptBlocked.ShouldBeFalse();
            z80.IFF1.ShouldBeFalse();
            z80.IFF2.ShouldBeFalse();
            z80.PrefixMode.ShouldBe(Z80Cpu.OpPrefixMode.None);
            z80.IndexMode.ShouldBe(Z80Cpu.OpIndexMode.None);
            z80.InterruptMode.ShouldBe((byte)0);
            z80.Tacts.ShouldBe(1000L);
            (z80.StateFlags & Z80StateFlags.Reset).ShouldBe(Z80StateFlags.None);
        }

        private class Z80TestMemoryDevice : IMemoryDevice
        {
            public byte OnReadMemory(ushort addr) => 0;

            public void OnWriteMemory(ushort addr, byte value) { }

            /// <summary>
            /// Gets the buffer that holds memory data
            /// </summary>
            /// <returns></returns>
            public byte[] GetMemoryBuffer() => null;
        }

        private class Z80TestPortDevice : IPortDevice
        {
            public byte OnReadPort(ushort addr) => 0xFF;
            public void OnWritePort(ushort addr, byte data) { }
            public void Reset() { }
        }
    }
}
