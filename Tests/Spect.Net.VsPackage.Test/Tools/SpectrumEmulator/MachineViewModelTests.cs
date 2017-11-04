using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Mvvm.Messages;

namespace Spect.Net.VsPackage.Test.Tools.SpectrumEmulator
{
    [TestClass]
    public class MachineViewModelTests
    {
        //[TestMethod]
        //public async Task StartDebugVmDoesNotInitializesSpectrumVmAfterPause()
        //{
        //    // --- Arrange
        //    var vm = new MachineViewModel
        //    {
        //        ClockProvider = new ClockProvider(),
        //        RomProvider = new ResourceRomProvider()
        //    };
        //    MakeSyncContext();
        //    vm.StartVmCommand.Execute(null);
        //    var before = vm.SpectrumVm;
        //    vm.PauseVmCommand.Execute(null);
        //    await vm.RunnerTask;
        //    MachineStateChangedMessage messageReceived = null;
        //    Messenger.Default.Register<MachineStateChangedMessage>(this,
        //        msg => { messageReceived = msg; });

        //    // --- Act
        //    MakeSyncContext();
        //    vm.StartDebugVmCommand.Execute(null);

        //    // --- Assert
        //    before.ShouldNotBeNull();
        //    vm.SpectrumVm.ShouldBeSameAs(before);
        //    vm.VmState.ShouldBe(VmState.Running);
        //    vm.RunnerTask.ShouldNotBeNull();
        //    messageReceived.NewState.ShouldBe(VmState.Running);
        //}

        //[TestMethod]
        //public async Task PauseVmDoesNotSendDebugMessageWhenNotDebugged()
        //{
        //    // --- Arrange
        //    var vm = new MachineViewModel
        //    {
        //        ClockProvider = new ClockProvider(),
        //        RomProvider = new ResourceRomProvider()
        //    };
        //    MakeSyncContext();
        //    vm.StartVmCommand.Execute(null);
        //    var before = vm.SpectrumVm;
        //    MachineDebugPausedMessage messageReceived = null;
        //    Messenger.Default.Register<MachineDebugPausedMessage>(this,
        //        msg => { messageReceived = msg; });

        //    // --- Act
        //    vm.PauseVmCommand.Execute(null);
        //    await vm.RunnerTask;

        //    // --- Assert
        //    before.ShouldNotBeNull();
        //    vm.SpectrumVm.ShouldBeSameAs(before);
        //    vm.VmState.ShouldBe(VmState.Paused);
        //    messageReceived.ShouldBeNull();
        //}

        //[TestMethod]
        //public async Task PauseVmSendsDebugMessageAfterStartDebug()
        //{
        //    // --- Arrange
        //    const ushort BREAK_ADDR = 0x0008;
        //    var debugInfo = new SpectrumDebugInfoProvider();
        //    debugInfo.Breakpoints.Add(BREAK_ADDR, MinimumBreakpointInfo.EmptyBreakpointInfo);
        //    var vm = new MachineViewModel
        //    {
        //        ClockProvider = new ClockProvider(),
        //        RomProvider = new ResourceRomProvider(),
        //        DebugInfoProvider = debugInfo
        //    };
        //    MakeSyncContext();
        //    vm.StartDebugVmCommand.Execute(null);
        //    var before = vm.SpectrumVm;
        //    MachineDebugPausedMessage messageReceived = null;
        //    Messenger.Default.Register<MachineDebugPausedMessage>(this,
        //        msg => { messageReceived = msg; });

        //    // --- Act
        //    vm.PauseVmCommand.Execute(null);
        //    await vm.RunnerTask;

        //    // --- Assert
        //    before.ShouldNotBeNull();
        //    vm.SpectrumVm.ShouldBeSameAs(before);
        //    vm.VmState.ShouldBe(VmState.Paused);
        //    vm.RunsInDebugMode.ShouldBeTrue();
        //    messageReceived.ShouldNotBeNull();
        //}

        //[TestMethod]
        //public async Task StepIntoSendsDebugMessageAfterCompleted()
        //{
        //    // --- Arrange
        //    const ushort BREAK_ADDR_1 = 0x0002;
        //    const ushort BREAK_ADDR_2 = 0x0005;
        //    var debugInfo = new SpectrumDebugInfoProvider();
        //    debugInfo.Breakpoints.Add(BREAK_ADDR_1, MinimumBreakpointInfo.EmptyBreakpointInfo);
        //    var vm = new MachineViewModel
        //    {
        //        ClockProvider = new ClockProvider(),
        //        RomProvider = new ResourceRomProvider(),
        //        DebugInfoProvider = debugInfo
        //    };
        //    MachineDebugPausedMessage messageReceived = null;
        //    Messenger.Default.Register<MachineDebugPausedMessage>(this,
        //        msg => { messageReceived = msg; });
        //    MakeSyncContext();
        //    vm.StartDebugVmCommand.Execute(null);
        //    await vm.RunnerTask;

        //    // --- Act
        //    MakeSyncContext();
        //    vm.StepIntoCommand.Execute(null);
        //    await vm.RunnerTask;

        //    // --- Assert
        //    vm.VmState.ShouldBe(VmState.Paused);
        //    vm.RunsInDebugMode.ShouldBeTrue();
        //    messageReceived.ShouldNotBeNull();
        //    vm.SpectrumVm.Cpu.Registers.PC.ShouldBe(BREAK_ADDR_2);
        //}

        //[TestMethod]
        //public async Task StepOverSendsDebugMessageAfterCompleted()
        //{
        //    // --- Arrange
        //    const ushort BREAK_ADDR_1 = 0x0002;
        //    const ushort BREAK_ADDR_2 = 0x0005;
        //    var debugInfo = new SpectrumDebugInfoProvider();
        //    debugInfo.Breakpoints.Add(BREAK_ADDR_1, MinimumBreakpointInfo.EmptyBreakpointInfo);
        //    var vm = new MachineViewModel
        //    {
        //        ClockProvider = new ClockProvider(),
        //        RomProvider = new ResourceRomProvider(),
        //        DebugInfoProvider = debugInfo
        //    };
        //    MachineDebugPausedMessage messageReceived = null;
        //    Messenger.Default.Register<MachineDebugPausedMessage>(this,
        //        msg => { messageReceived = msg; });
        //    MakeSyncContext();
        //    vm.StartDebugVmCommand.Execute(null);
        //    await vm.RunnerTask;

        //    // --- Act
        //    MakeSyncContext();
        //    vm.StepOverCommand.Execute(null);
        //    await vm.RunnerTask;

        //    // --- Assert
        //    vm.VmState.ShouldBe(VmState.Paused);
        //    vm.RunsInDebugMode.ShouldBeTrue();
        //    messageReceived.ShouldNotBeNull();
        //    vm.SpectrumVm.Cpu.Registers.PC.ShouldBe(BREAK_ADDR_2);
        //}

        //private void MakeSyncContext()
        //{
        //    // --- We need a synchronization context, as the 
        //    // --- MachineViewModel uses it
        //    SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        //}
    }
}
