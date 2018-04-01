using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Scripting;

namespace Spect.Net.SpectrumEmu.Test.Scripting
{
    [TestClass]
    public class AddressTrackingStateTests
    {
        [TestMethod]
        public async Task AddressTrackingWorks1()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();

            // --- Act
            sm.Breakpoints.AddBreakpoint(0x0001);
            sm.StartDebug();
            await sm.CompletionTask;
            
            // --- Assert
            sm.Cpu.OperationTrackingState[0x0000].ShouldBeTrue();
            sm.Cpu.OperationTrackingState.TouchedAny(0x0001, 0xFFFF).ShouldBeFalse();
        }

        [TestMethod]
        public async Task AddressTrackingWorks2()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();

            // --- Act
            sm.Breakpoints.AddBreakpoint(0x11CE);
            sm.StartDebug();
            await sm.CompletionTask;

            // --- Assert
            sm.Cpu.OperationTrackingState.TouchedAll(0x0000, 0x0007).ShouldBeTrue();
            sm.Cpu.OperationTrackingState.TouchedAll(0x11CB, 0x11CD).ShouldBeTrue();
            sm.Cpu.OperationTrackingState.TouchedAny(0x0008, 0x11CA).ShouldBeFalse();
            sm.Cpu.OperationTrackingState.TouchedAny(0x11CE, 0xFFFF).ShouldBeFalse();
        }

        [TestMethod]
        public async Task ResetOperationTrackingWorks()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            sm.Breakpoints.AddBreakpoint(0x11CB);
            sm.StartDebug();
            await sm.CompletionTask;
            var pcBefore = sm.Cpu.PC;
            sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.BreakpointReached);
            sm.MachineState.ShouldBe(VmState.Paused);

            // --- Act
            sm.Cpu.ResetOperationTracking();
            sm.Breakpoints.ClearAllBreakpoints();
            sm.Breakpoints.AddBreakpoint(0x11CE);
            sm.StartDebug();
            await sm.CompletionTask;
            var pcAfter = sm.Cpu.PC;

            // --- Assert
            pcBefore.ShouldBe((ushort)0x11CB);
            pcAfter.ShouldBe((ushort)0x11CE);
            sm.Cpu.OperationTrackingState.TouchedAny(0x0000, 0x11CA).ShouldBeFalse();
            sm.Cpu.OperationTrackingState[0x11CB].ShouldBeTrue();
            sm.Cpu.OperationTrackingState[0x11CC].ShouldBeTrue();
            sm.Cpu.OperationTrackingState[0x11CD].ShouldBeTrue();
            sm.Cpu.OperationTrackingState.TouchedAny(0x11CE, 0xFFFF).ShouldBeFalse();
        }
    }
}
