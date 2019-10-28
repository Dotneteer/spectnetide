using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Cpu;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Devices.Interrupt;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Interrupt
{
    [TestClass]
    public class InterruptDeviceTests
    {
        private const int TEST_INT_TACT = 13;

        [TestMethod]
        public void DisabledInterruptIsNotRaised()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xED, 0x56,       // IM 1
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x00, 0x0A, // LD BC,$0A00
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800F);

            spectrum.Cpu.Tacts.ShouldBeGreaterThanOrEqualTo(66599L);
        }

        [TestMethod]
        public void EnabledInterruptIsRaised()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            // --- We render the screen while the interrupt is enabled
            spectrum.InitCode(new byte[]
            {
                0xED, 0x56,       // IM 1
                0xFB,             // EI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x00, 0x0A, // LD BC,$0A00
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800F);

            // --- The instructions above take 66599 tacts while reaching the HALT operation
            // --- However, an interrupt is generated, and because of IM 1, the RST 38 is
            // --- invoked. It checks to keyboard status in 1034 tacts.
            // --- When HALT is reached, the CPU tact count is 67633.
            spectrum.Cpu.Tacts.ShouldBeGreaterThanOrEqualTo(67553L);
        }

        [TestMethod]
        public void InterruptDeviceResetWorksAsExpected()
        {
            // --- Arrange
            var vm = new SpectrumAdvancedTestMachine();
            var idev = new InterruptDevice(TEST_INT_TACT);
            idev.OnAttachedToVm(vm);

            // --- Act
            idev.Reset();

            // --- Assert
            idev.InterruptRaised.ShouldBeFalse();
            idev.InterruptRevoked.ShouldBeFalse();
        }

        [TestMethod]
        public void InterruptNotRaisedTooEarly()
        {
            // --- Arrange
            var vm = new SpectrumAdvancedTestMachine();
            var idev = new InterruptDevice(TEST_INT_TACT);
            idev.OnAttachedToVm(vm);

            // --- Act/Assert
            for (var tact = 0; tact < TEST_INT_TACT; tact++)
            {
                idev.CheckForInterrupt(tact);
                idev.InterruptRaised.ShouldBeFalse();
                idev.InterruptRevoked.ShouldBeFalse();
                idev.FrameCount.ShouldBe(0);
                (vm.Cpu.StateFlags & Z80StateFlags.Int).ShouldBe(Z80StateFlags.None);
            }
        }

        [TestMethod]
        public void InterruptIsNotRaisedTooLate()
        {
            // --- Arrange
            var vm = new SpectrumAdvancedTestMachine();
            var idev = new InterruptDevice(TEST_INT_TACT);
            idev.OnAttachedToVm(vm);

            // --- Act/Assert
            var lateTact = TEST_INT_TACT + InterruptDevice.LONGEST_OP_TACTS + 1;
            for (var tact = lateTact; tact < lateTact + 10; tact++)
            {
                idev.CheckForInterrupt(tact);
                idev.InterruptRaised.ShouldBeFalse();
                idev.InterruptRevoked.ShouldBeTrue();
                idev.FrameCount.ShouldBe(0);
                (vm.Cpu.StateFlags & Z80StateFlags.Int).ShouldBe(Z80StateFlags.None);
            }
        }

        [TestMethod]
        public void InterruptIsRaisedWithinAllowedRange()
        {
            for (var tact = TEST_INT_TACT; tact <= TEST_INT_TACT + InterruptDevice.LONGEST_OP_TACTS; tact++)
            {
                // --- Arrange
                var vm = new SpectrumAdvancedTestMachine();
                var idev = new InterruptDevice(TEST_INT_TACT);
                idev.OnAttachedToVm(vm);

                // --- Act/Assert
                idev.CheckForInterrupt(tact);
                idev.InterruptRaised.ShouldBeTrue();
                idev.InterruptRevoked.ShouldBeFalse();
                idev.FrameCount.ShouldBe(1);
                (vm.Cpu.StateFlags & Z80StateFlags.Int).ShouldBe(Z80StateFlags.Int);
            }
        }

        [TestMethod]
        public void InterruptIsNotRereaised()
        {
            // --- Arrange
            var vm = new SpectrumAdvancedTestMachine();
            var idev = new InterruptDevice(TEST_INT_TACT);
            idev.OnAttachedToVm(vm);
            idev.CheckForInterrupt(TEST_INT_TACT);

            // --- Act
            vm.Cpu.StateFlags &= Z80StateFlags.InvInt;
            idev.CheckForInterrupt(TEST_INT_TACT + 1);

            // --- Assert
            idev.InterruptRaised.ShouldBeTrue();
            idev.InterruptRevoked.ShouldBeFalse();
            (vm.Cpu.StateFlags & Z80StateFlags.Int).ShouldBe(Z80StateFlags.None);
            idev.FrameCount.ShouldBe(1);
        }
    }
}
