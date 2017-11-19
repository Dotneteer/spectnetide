using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
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
            z80.Tacts.ShouldBe(0);
            (z80.StateFlags & Z80StateFlags.Reset).ShouldBe(Z80StateFlags.None);
        }

        [TestMethod]
        public void MaskableInterruptModeIsReached()
        {
            // --- Arrange
            var z80 = new Z80Cpu(new Z80SimpleMemoryDevice(), new Z80TestPortDevice());
            z80.IFF1 = z80.IFF2 = true;
            z80.Registers.SP = 0x100;

            // --- Act
            z80.StateFlags |= Z80StateFlags.Int;
            z80.ExecuteCpuCycle();

            // --- Assert
            z80.MaskableInterruptModeEntered.ShouldBeTrue();
        }

        [TestMethod]
        public void MaskableInterruptModeIsLeft()
        {
            // --- Arrange
            var z80 = new Z80Cpu(new Z80SimpleMemoryDevice(), new Z80TestPortDevice());
            z80.IFF1 = z80.IFF2 = true;
            z80.Registers.SP = 0x100;
            z80.StateFlags |= Z80StateFlags.Int;
            z80.ExecuteCpuCycle();

            // --- Act
            z80.ExecuteCpuCycle();

            // --- Assert
            z80.MaskableInterruptModeEntered.ShouldBeFalse();
        }

        private class Z80TestMemoryDevice : IMemoryDevice
        {
            public int AddressableSize => 0x1_0000;

            public byte Read(ushort addr) => 0;

            public void Write(ushort addr, byte value) { }

            /// <summary>
            /// Gets the buffer that holds memory data
            /// </summary>
            /// <returns></returns>
            public byte[] CloneMemory() => null;

            /// <summary>
            /// The ULA reads the memory at the specified address
            /// </summary>
            /// <param name="addr">Memory address</param>
            /// <returns>Byte read from the memory</returns>
            /// <remarks>
            /// We need this device to emulate the contention for the screen memory
            /// between the CPU and the ULA.
            /// </remarks>
            public byte UlaRead(ushort addr)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Fills up the memory from the specified buffer
            /// </summary>
            /// <param name="buffer">Contains the row data to fill up the memory</param>
            public void CopyRom(byte[] buffer)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Resets this device
            /// </summary>
            public void Reset()
            {
            }

            /// <summary>
            /// The virtual machine that hosts the device
            /// </summary>
            public ISpectrumVm HostVm { get; set; }

            /// <summary>
            /// Signs that the device has been attached to the Spectrum virtual machine
            /// </summary>
            public void OnAttachedToVm(ISpectrumVm hostVm)
            {
            }
        }

        private class Z80TestPortDevice : IPortDevice
        {
            public byte OnReadPort(ushort addr) => 0xFF;
            public void OnWritePort(ushort addr, byte data) { }
            public void Reset() { }

            /// <summary>
            /// The virtual machine that hosts the device
            /// </summary>
            public ISpectrumVm HostVm { get; set; }

            /// <summary>
            /// Signs that the device has been attached to the Spectrum virtual machine
            /// </summary>
            public void OnAttachedToVm(ISpectrumVm hostVm)
            {
            }
        }

        private class Z80SimpleMemoryDevice : IMemoryDevice
        {
            private readonly byte[] _buffer = new byte[1024];

            public Z80SimpleMemoryDevice()
            {
                for (var i = 0; i < _buffer.Length; i++)
                {
                    _buffer[i] = 0x00;
                }
            }

            public int AddressableSize => 0x0400;

            public byte Read(ushort addr) => _buffer[addr];

            public void Write(ushort addr, byte value)
            {
                _buffer[addr] = value;
            }

            /// <summary>
            /// Gets the buffer that holds memory data
            /// </summary>
            /// <returns></returns>
            public byte[] CloneMemory() => null;

            /// <summary>
            /// The ULA reads the memory at the specified address
            /// </summary>
            /// <param name="addr">Memory address</param>
            /// <returns>Byte read from the memory</returns>
            /// <remarks>
            /// We need this device to emulate the contention for the screen memory
            /// between the CPU and the ULA.
            /// </remarks>
            public byte UlaRead(ushort addr)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Fills up the memory from the specified buffer
            /// </summary>
            /// <param name="buffer">Contains the row data to fill up the memory</param>
            public void CopyRom(byte[] buffer)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Resets this device
            /// </summary>
            public void Reset()
            {
            }

            /// <summary>
            /// The virtual machine that hosts the device
            /// </summary>
            public ISpectrumVm HostVm { get; set; }

            /// <summary>
            /// Signs that the device has been attached to the Spectrum virtual machine
            /// </summary>
            public void OnAttachedToVm(ISpectrumVm hostVm)
            {
            }
        }
    }
}
