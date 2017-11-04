using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Mvvm.Messages;
using Spect.Net.Wpf.Providers;

namespace Spect.Net.Wpf.Test.SpectrumControl
{
    [TestClass]
    public class MachineViewModelTests
    {
        [TestMethod]
        public void ConstructorInitializesVmProperly()
        {
            // --- Act
            var vm = new MachineViewModel();

            // --- Assert
            vm.DisplayMode.ShouldBe(SpectrumDisplayMode.Fit);
            vm.VmState.ShouldBe(VmState.None);
            vm.StartVmCommand.ShouldNotBeNull();
            vm.PauseVmCommand.ShouldNotBeNull();
            vm.StopVmCommand.ShouldNotBeNull();
            vm.ResetVmCommand.ShouldNotBeNull();
            vm.CancellationTokenSource.ShouldBeNull();
            vm.SpectrumVm.ShouldBeNull();
        }

        [TestMethod]
        public void StartVmInitializesSpectrumVmFirstTime()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            var before = vm.SpectrumVm;
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);

            // --- Assert
            before.ShouldBeNull();
            vm.SpectrumVm.ShouldNotBeNull();
            vm.VmState.ShouldBe(VmState.Running);
            vm.RunsInDebugMode.ShouldBeFalse();
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(VmState.Running);
        }

        [TestMethod]
        public async Task PauseVmShouldSuspendSpectrumVm()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.PauseVmCommand.Execute(null);
            await vm.RunnerTask;

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(VmState.Paused);
            messageReceived.NewState.ShouldBe(VmState.Paused);
        }

        [TestMethod]
        public async Task StartVmDoesNotInitializesSpectrumVmAfterPause()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            vm.PauseVmCommand.Execute(null);
            await vm.RunnerTask;
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(VmState.Running);
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(VmState.Running);
        }

        [TestMethod]
        public void PauseVmIgnoresNotStartedSpectrumVm()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.PauseVmCommand.Execute(null);


            // --- Assert
            vm.SpectrumVm.ShouldBeNull();
            vm.VmState.ShouldBe(VmState.None);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public async Task StopVmWorksAsExpected()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);

            // --- Act
            vm.StopVmCommand.Execute(null);
            await vm.RunnerTask;

            // --- Assert
            vm.VmState.ShouldBe(VmState.Stopped);
            messageReceived.NewState.ShouldBe(VmState.Stopped);
        }

        [TestMethod]
        public async Task PauseVmIgnoresStoppedSpectrumVm()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);
            vm.StopVmCommand.Execute(null);
            await vm.RunnerTask;
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            MakeSyncContext();
            vm.PauseVmCommand.Execute(null);
            await vm.RunnerTask;

            // --- Assert
            vm.VmState.ShouldBe(VmState.Stopped);
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void StopVmIgnoresNotStartedVm()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.StopVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldBeNull();
            vm.VmState.ShouldBe(VmState.None);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void ResetVmIgnoresNotStartedVm()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.ResetVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldBeNull();
            vm.VmState.ShouldBe(VmState.None);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public async Task ResetVmStartsAndResetsPausedVm()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);
            vm.PauseVmCommand.Execute(null);
            await vm.RunnerTask;

            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });
            var before = vm.SpectrumVm;

            // --- Act
            MakeSyncContext();
            vm.ResetVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(VmState.Running);
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(VmState.Running);
        }

        [TestMethod]
        public void ResetVmWorksWithRunningVm()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });
            var before = vm.SpectrumVm;

            // --- Act
            vm.ResetVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(VmState.Running);
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public async Task ResetVmIgnoresStoppedVm()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);
            vm.StopVmCommand.Execute(null);
            await vm.RunnerTask;
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.ResetVmCommand.Execute(null);

            // --- Assert
            vm.VmState.ShouldBe(VmState.Stopped);
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void SetZoomNotifiesAboutDisplayModeChange()
        {
            // --- Act
            var vm = new MachineViewModel {DisplayMode = SpectrumDisplayMode.Fit};
            MachineDisplayModeChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineDisplayModeChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.DisplayMode = SpectrumDisplayMode.Zoom2;

            // --- Assert
            messageReceived.DisplayMode.ShouldBe(SpectrumDisplayMode.Zoom2);
        }

        [TestMethod]
        public void StartDebugVmInitializesSpectrumVmFirstTime()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            var before = vm.SpectrumVm;
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            MakeSyncContext();
            vm.StartDebugVmCommand.Execute(null);

            // --- Assert
            before.ShouldBeNull();
            vm.SpectrumVm.ShouldNotBeNull();
            vm.VmState.ShouldBe(VmState.Running);
            vm.RunsInDebugMode.ShouldBeTrue();
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(VmState.Running);
        }

        [TestMethod]
        public async Task StartDebugVmDoesNotInitializesSpectrumVmAfterPause()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            vm.PauseVmCommand.Execute(null);
            await vm.RunnerTask;
            MachineStateChangedMessage messageReceived = null;
            Messenger.Default.Register<MachineStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            MakeSyncContext();
            vm.StartDebugVmCommand.Execute(null);

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(VmState.Running);
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(VmState.Running);
        }

        [TestMethod]
        public async Task PauseVmDoesNotSendDebugMessageWhenNotDebugged()
        {
            // --- Arrange
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            MakeSyncContext();
            vm.StartVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            MachineDebugPausedMessage messageReceived = null;
            Messenger.Default.Register<MachineDebugPausedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.PauseVmCommand.Execute(null);
            await vm.RunnerTask;

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(VmState.Paused);
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public async Task PauseVmSendsDebugMessageAfterStartDebug()
        {
            // --- Arrange
            const ushort BREAK_ADDR = 0x0008;
            var debugInfo = new SpectrumDebugInfoProvider();
            debugInfo.Breakpoints.Add(BREAK_ADDR, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider(),
                DebugInfoProvider = debugInfo
            };
            MakeSyncContext();
            vm.StartDebugVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            MachineDebugPausedMessage messageReceived = null;
            Messenger.Default.Register<MachineDebugPausedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.PauseVmCommand.Execute(null);
            await vm.RunnerTask;

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(VmState.Paused);
            vm.RunsInDebugMode.ShouldBeTrue();
            messageReceived.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task StepIntoSendsDebugMessageAfterCompleted()
        {
            // --- Arrange
            const ushort BREAK_ADDR_1 = 0x0002;
            const ushort BREAK_ADDR_2 = 0x0005;
            var debugInfo = new SpectrumDebugInfoProvider();
            debugInfo.Breakpoints.Add(BREAK_ADDR_1, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider(),
                DebugInfoProvider = debugInfo
            };
            MachineDebugPausedMessage messageReceived = null;
            Messenger.Default.Register<MachineDebugPausedMessage>(this,
                msg => { messageReceived = msg; });
            MakeSyncContext();
            vm.StartDebugVmCommand.Execute(null);
            await vm.RunnerTask;

            // --- Act
            MakeSyncContext();
            vm.StepIntoCommand.Execute(null);
            await vm.RunnerTask;

            // --- Assert
            vm.VmState.ShouldBe(VmState.Paused);
            vm.RunsInDebugMode.ShouldBeTrue();
            messageReceived.ShouldNotBeNull();
            vm.SpectrumVm.Cpu.Registers.PC.ShouldBe(BREAK_ADDR_2);
        }

        [TestMethod]
        public async Task StepOverSendsDebugMessageAfterCompleted()
        {
            // --- Arrange
            const ushort BREAK_ADDR_1 = 0x0002;
            const ushort BREAK_ADDR_2 = 0x0005;
            var debugInfo = new SpectrumDebugInfoProvider();
            debugInfo.Breakpoints.Add(BREAK_ADDR_1, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var vm = new MachineViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider(),
                DebugInfoProvider = debugInfo
            };
            MachineDebugPausedMessage messageReceived = null;
            Messenger.Default.Register<MachineDebugPausedMessage>(this,
                msg => { messageReceived = msg; });
            MakeSyncContext();
            vm.StartDebugVmCommand.Execute(null);
            await vm.RunnerTask;

            // --- Act
            MakeSyncContext();
            vm.StepOverCommand.Execute(null);
            await vm.RunnerTask;

            // --- Assert
            vm.VmState.ShouldBe(VmState.Paused);
            vm.RunsInDebugMode.ShouldBeTrue();
            messageReceived.ShouldNotBeNull();
            vm.SpectrumVm.Cpu.Registers.PC.ShouldBe(BREAK_ADDR_2);
        }

        private void MakeSyncContext()
        {
            // --- We need a synchronization context, as the 
            // --- MachineViewModel uses it
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }
    }
}
